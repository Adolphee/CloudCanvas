using CloudCanvas.Application.Common.Exceptions;
using System.Text.Json;

namespace CloudCanvas.Application.Common
{
    public class CCSerializer
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public static string Serialize(object target) => JsonSerializer.Serialize(Validate.Object(target), _options);
        public static string Serialize(FileMetadata target) => JsonSerializer.Serialize((target), _options);
        
        /// <summary>
        /// Tries to deserialize a structured string into an object of a given type.
        /// </summary>
        /// <typeparam name="T">The type of the object to try and create.</typeparam>
        /// <param name="blobMetadataJson"></param>
        /// <returns></returns>
        /// <exception cref="CCSerializationException">When the operation fails</exception>
        public static T Deserialize<T>(string blobMetadataJson)
        {
            try
            {
                Validate.StringValue(nameof(blobMetadataJson), blobMetadataJson);
                T? dto = JsonSerializer.Deserialize<T>(blobMetadataJson, _options);
                return Validate.Object(dto);
            }
            catch (Exception e) when (e is JsonException || e is  InvalidArgumentException || e is NotSupportedException)
            {
                throw new CCSerializationException($"Invalid argument '{nameof(blobMetadataJson)}' provided with value: '{blobMetadataJson}'.", e);
            }
        }
    }
}
