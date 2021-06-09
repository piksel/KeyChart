using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KeyChart.GUI.Util
{
    public class CachedApiClient
    {
        public CachedApiClient(DirectoryInfo cacheDir, IJsonSerializer json)
        {
            CacheDir = cacheDir;
            Json = json;
        }

        IJsonSerializer Json { get; }
        
        private FileInfo CacheFile(string keyDir, string keyId) 
            => new (Path.Combine(CacheDir.FullName, keyDir, CleanFileName(keyId)));

        private string CleanFileName(string fileName)
        {
            var badChars = Path.GetInvalidFileNameChars();
            return string.Concat(fileName.Select(c => badChars.Contains(c) ? '_' : c));
        }

        private HttpClient _http = new();
        private DirectoryInfo CacheDir { get; }

        public async ValueTask<TData?> GetJson<TData>(FormattableString url, string key, CancellationToken token) 
            => url.ArgumentCount < 1 
                ? await GetJson<TData>(url.ToString(), "single" , key, token)
                : await GetJson<TData>(url.ToString(), key, 
                    string.Join('_', url.GetArguments().Select(o => o?.ToString() ?? "-")), token);

        private async ValueTask<TData?> GetJson<TData>(string url, string keyDir, string keyId, CancellationToken token)
        {
            var cacheFile = CacheFile(keyDir, $"{keyId}.json");
            cacheFile.Directory?.Create();
            if (cacheFile.Exists)
            {
                await using var fileStream = cacheFile.OpenRead();
                return await Json.DeserializeAsync<TData>(fileStream, token);
            }
            else
            {
                // ReSharper disable once CoVariantArrayConversion  
                await using var httpStream = await _http.GetStreamAsync(url, token);
                await using var fileStream = cacheFile.Create();
                await httpStream.CopyToAsync(fileStream, token);
                return await Json.DeserializeAsync<TData>(fileStream, token);
            }
        }
    }
}