using Microsoft.AspNetCore.Mvc;
using CloudCanvas.Shared.Services;
using CloudCanvas.Web.Models;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;

namespace CloudCanvas.Web.Controllers
{
    [Route("[controller]")]
    public class GalleryController : Controller
    {
        private readonly ILogger<GalleryController> _logger;
        private readonly IBlobStorageService _service;
        public List<string> ImageLinks { get; set; } = new List<string>();

        public GalleryController(ILogger<GalleryController> logger, BlobStorageService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ImageLinks = await _service.GetBlobUrlsAsync(BlobStorage.Containers.Uploads);
            return View(nameof(Index), new GalleryViewModel { ImageLinks = ImageLinks });
        }

        [HttpGet("gallery/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}