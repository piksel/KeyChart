using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using KeyChart.GUI.Util;
using Nuke.Common.IO;

namespace KeyChart.GUI
{
    public class DataStore<TData> where TData: new()
    {
        public TData? Data { get; set; }

        public Func<TData> DefaultFactory { get; set; } = () => new TData();
        
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
                if (fileStream.Length == 0) return default;
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

        public void Mutate(Action<TData> change)
        {
            var data = Data ?? DefaultFactory();
            change(data);
            Data = data;
        }
    }
}