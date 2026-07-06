using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudCanvas.Infrastructure.Common
{
    public sealed class PostJsonConverter : JsonConverter<IPost>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override IPost? ReadJson(JsonReader reader, Type objectType, IPost? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default!;

            var jo = JObject.Load(reader);

            var classificationToken = jo["classification"] ?? jo["Classification"];
            if (classificationToken is null)
            {
                throw new JsonSerializationException(
                    "Missing 'classification' discriminator for IPost.");
            }

            PostClassification classification;
            try
            {
                classification = classificationToken.Type switch
                {
                    JTokenType.String => Enum.Parse<PostClassification>(
                        classificationToken.Value<string>()!,
                        ignoreCase: true),

                    JTokenType.Integer => (PostClassification)classificationToken.Value<int>(),

                    _ => throw new JsonSerializationException(
                        $"Unsupported classification token type: {classificationToken.Type}")
                };
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException(
                    $"Invalid classification value '{classificationToken}'.", ex);
            }

            IPost target = classification switch
            {
                PostClassification.Photo => new Photo(),
                PostClassification.Gallery => new Gallery(),
                //PostClassification.Comment => new Comment(),
                //PostClassification.Video => new Video(),
                _ => throw new JsonSerializationException(
                    $"Unsupported post classification '{classification}'.")
            };

            using var subReader = jo.CreateReader();
            serializer.Populate(subReader, target);

            return target;
        }

        public override void WriteJson(JsonWriter writer, IPost value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, value, value.GetType());
        }
    }

}
