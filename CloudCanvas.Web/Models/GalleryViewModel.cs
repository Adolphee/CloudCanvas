using Microsoft.AspNetCore.Mvc;

namespace CloudCanvas.Web.Models
{
    public class GalleryViewModel
    {
        [BindProperty]
        public required List<string> ImageLinks { get; set; }
    }
}