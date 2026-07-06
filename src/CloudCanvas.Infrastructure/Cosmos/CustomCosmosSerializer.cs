using CloudCanvas.Infrastructure.Common;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Infrastructure.Cosmos
{
    public class CustomCosmosSerializer : CosmosSerializer
    {
        private readonly JsonSerializer _serializer;

        public CustomCosmosSerializer(JsonSerializerSettings settings, CosmosSerializationOptions options)
        {
            _serializer = JsonSerializer.Create(settings);
            //_serializer.Converters.Add(new PostJsonConverter());
        }

        public CustomCosmosSerializer() {
            _serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            //_serializer.Converters.Add(new PostJsonConverter());
        }

        public override T FromStream<T>(Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return default!;

            using var sr = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(sr);

            return _serializer.Deserialize<T>(jsonTextReader)!;
        }

        public override Stream ToStream<T>(T input)
        {
            var stream = new MemoryStream();
            using (var sw = new StreamWriter(stream, leaveOpen: true))
            using (var writer = new JsonTextWriter(sw))
            {
                _serializer.Serialize(writer, input);
                writer.Flush();
                sw.Flush();
            }

            stream.Position = 0;
            return stream;
        }
    }
}
