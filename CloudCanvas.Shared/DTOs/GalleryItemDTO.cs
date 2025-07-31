using Azure.Storage.Blobs.Models;
using CloudCanvas.Shared.Enums;
using System.ComponentModel.DataAnnotations;
namespace CloudCanvas.Shared.DTOs
{
    public class GalleryItemDTO: PatchGalleryItemDTO
    {
        [Required]
        public string Url { get; set; } = default!;
        public Dictionary<ThumbnailSize, string> Thumbnails { get; set; } = new(); // BlobUrls for the thumbnails
        public string? UploadedBy { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public long ContentLength { get; set; }
    }
}
