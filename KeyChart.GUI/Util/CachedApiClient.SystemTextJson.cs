﻿using System;
using System.IO;
using System.Text;
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

        public static IJsonSerializer SnakeCaseSerializer => new SystemTextJsonSerializer(
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
    }

    public class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        StringBuilder sb = new StringBuilder();
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            sb.Clear();
            
            for (int i = 0; i < name.Length; i++)
            {
                if (i > 2 && char.IsUpper(name, i) && !char.IsUpper(name, i - 1))
                {
                    sb.Append('_');
                }

                sb.Append(char.ToLowerInvariant(name[i]));
            }

            var snakeName = sb.ToString();

            return snakeName;
        }
    }
}