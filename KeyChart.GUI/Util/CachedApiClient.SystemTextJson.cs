using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KeyChart.GUI.Util
{
    public record SystemTextJsonSerializer(JsonSerializerOptions? SerializerOptions = null): IJsonSerializer
    {
        public async ValueTask<TData?> DeserializeAsync<TData>(Stream stream, CancellationToken token) 
            => await JsonSerializer.DeserializeAsync<TData>(stream, SerializerOptions, token);

        public async Task SerializeAsync<TData>(Stream stream, TData data, CancellationToken token) 
            => await JsonSerializer.SerializeAsync(stream, data, SerializerOptions, token);
    }
}