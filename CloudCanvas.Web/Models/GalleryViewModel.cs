using CloudCanvas.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CloudCanvas.Web.Models
{
    public class GalleryViewModel
    {
        [BindProperty]
        public required List<BlobMetaDTO> Blobs { get; set; }
    }
}