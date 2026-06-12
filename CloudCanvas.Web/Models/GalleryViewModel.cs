using CloudCanvas.Shared.DTOs;
using CloudCanvas.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace CloudCanvas.Web.Models
{
    public class GalleryViewModel
    {
        [BindProperty]
        public required List<BlobMetaDTO> Blobs { get; set; } = new();
        public required List<Photo> Photos { get; set; } = new();

        public required ApplicationUser curentUser { get; set; }

    }
}