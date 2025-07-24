using Microsoft.AspNetCore.Mvc;
using CloudCanvas.Shared.Services;
using CloudCanvas.Web.Models;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.DTOs;

namespace CloudCanvas.Web.Controllers
{
    [Route("[controller]")]
    public class GalleryController : Controller
    {
        private readonly ILogger<GalleryController> _logger;
        private readonly IBlobStorageService _bservice;
        public List<BlobMetaDTO> BlobsMetadataList { get; set; } = new();
        public List<string> BlobUrls { get; set; } = new();

        public GalleryController(ILogger<GalleryController> logger, BlobStorageService bservice)
        {
            _logger = logger;
            _bservice = bservice;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("[GET] Processing request: Getting blob urls from {service}...", nameof(BlobStorageService));
            BlobUrls = await _bservice.GetBlobUrlsAsync(BlobStorage.Containers.Uploads);
            _logger.LogInformation("[GET] Succesfully obtained blob urls from {service}...", nameof(BlobStorageService));
            return View(nameof(Index), new GalleryViewModel { 
                BlobUrls = BlobUrls,
                BlobsMetadata = BlobsMetadataList
            });
        }

        [HttpGet("gallery/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}