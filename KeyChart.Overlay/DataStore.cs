using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using KeyChart.Avalonia.Util;
using Nuke.Common.IO;

namespace KeyChart.Avalonia
{
    public class DataStore<TData>
    {
        public TData? Data { get; set; }

        
        private FileInfo DataFile { get; }
        private IJsonSerializer Serializer { get; }
        
        
        public DataStore([NotNull] AbsolutePath storeFilePath, IJsonSerializer? serializer = null)
        {
            DataFile = new FileInfo(storeFilePath);
            Serializer = serializer ?? new SystemTextJsonSerializer(
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        }

        public async ValueTask<TData?> Load(CancellationToken? token = null)
        {
            if (!DataFile.Exists) return default;

            try
            {
                await using var fileStream = DataFile.OpenRead();
                return Data = await Serializer.DeserializeAsync<TData>(fileStream, token ?? CancellationToken.None);
            }
            catch (JsonException)
            {
#if DEBUG
                Debugger.Break();
#endif
                return default;
            }
        }
        
        public async Task Save(CancellationToken? token = null)
        {
            if (Data is null) return;

            await using var fileStream = DataFile.Open(FileMode.Create);
            await Serializer.SerializeAsync<TData>(fileStream, Data, token ?? CancellationToken.None);
            fileStream.Flush(true);
        }

        public async Task<TData> LoadOrDefault(Func<TData> defaultInit)
        {
            await Load(CancellationToken.None);
            return Data ??= defaultInit();
        }
    }
}