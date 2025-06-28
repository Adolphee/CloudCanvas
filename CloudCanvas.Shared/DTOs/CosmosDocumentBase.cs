using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.DTOs
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
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
