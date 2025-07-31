using Azure.Storage.Blobs.Models;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.DTOs
{
    public class PatchGalleryItemDTO: MetadataDocumentBase
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("project")]
        public string? Project { get; set; }
        [JsonPropertyName("galleryName")]
        public string? GalleryName { get; set; }
        [JsonPropertyName("userTags")]
        public List<string>? UserTags { get; set; } = default!;
    }
}
