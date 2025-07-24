using Azure.Storage.Blobs.Models;
using CloudCanvas.Shared.Enums;
using System.ComponentModel.DataAnnotations;
namespace CloudCanvas.Shared.DTOs
{
    public class GalleryItemDTO: MetadataDocumentBase
    {
        [Required, MaxLength(100)]
        public string OriginalFileName { get; set; } = default!;
        [Required, MaxLength(255)]
        public string Url { get; set; } = default!;
        [Required, Range(0, 4)]
        public int ProcessingStage { get; set; } = default!;
        public string? UploadedBy { get; set; } = default!;
        [Required]
        public string ContainerName { get; set; } = default!;
        [Required]
        public string Name { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public string? Project { get; set; } = default!;
        public string? GalleryName { get; set; } = default!;
        public Dictionary<ThumbnailSize, string> Thumbnails { get; set; } = new(); // BlobUrls for the thumbnails
        public List<string> Tags { get; set; } = new(); // for future A.I. integration for auto-tagging
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public BlobType BlobType { get; set; }
        public string? ContentType { get; set; }
        public long TagCount { get; set; }
        public long ContentLength { get; set; }
        public string? ContentEncoding { get; set; }
        public string? ContentLanguage { get; set; }
    }
}
