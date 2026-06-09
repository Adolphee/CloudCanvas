using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CloudCanvas.Shared.DTOs
{
    /// <summary>
    /// Represents the base class for metadata documents stored in the Cosmos database.
    /// </summary>
    /// <remarks>This class provides a common structure for metadata documents, including the partition-key (the associated user).
    /// It is intended to be inherited by specific metadata document types.</remarks>
    public abstract class MetadataDocumentBase: CosmosDocumentBase
    {
        [Required]
        [JsonPropertyName("userId")]
        public string? UserId { get; set; } // should be overwritten for more structured Id's
    }
}
