using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CloudCanvas.Application.Posts.DTOs
{
    /// <summary>
    /// Represents the base class for a Cosmos DB document, providing a unique identifier.
    /// </summary>
    /// <remarks>This class serves as a base for documents stored in Cosmos DB. The <see cref="Id"/> property
    /// is required and uniquely identifies the document within a Cosmos DB container.</remarks>
    public record CosmosDocumentBase
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; init; } = default!;

        // soft deletes; null = not deleted
        [JsonIgnore]
        public DateTimeOffset? DeletedOn { get; init; } = null;

        [Required]
        [JsonPropertyName("containerName")]
        public string ContainerName { get; init; } = BStorage.Containers.Uploads;
    }
}
