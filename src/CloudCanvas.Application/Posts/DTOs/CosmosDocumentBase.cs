using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CloudCanvas.Application.Posts.DTOs
{
    /// <summary>
    /// Represents the base class for a Cosmos DB document, providing a unique identifier.
    /// </summary>
    /// <remarks>This class serves as a base for documents stored in Cosmos DB. The <see cref="Id"/> property
    /// is required and uniquely identifies the document within a Cosmos DB container.</remarks>
    public class CosmosDocumentBase
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        // soft deletes; null = not deleted
        [JsonIgnore]
        public DateTimeOffset? DeletedOn { get; set; } = null;

        [Required]
        [JsonPropertyName("containerName")]
        public string ContainerName { get; set; } = BStorage.Containers.Uploads;
    }
}
