using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KeyChart.Avalonia.Util
{
    public interface IJsonSerializer
    {
        public ValueTask<TData?> DeserializeAsync<TData>(Stream stream, CancellationToken token);
        public Task SerializeAsync<TData>(Stream stream, TData data, CancellationToken token);  
    }
}