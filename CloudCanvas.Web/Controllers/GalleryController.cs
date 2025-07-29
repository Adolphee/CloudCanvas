using Microsoft.AspNetCore.Mvc;
using CloudCanvas.Shared.Services;
using CloudCanvas.Web.Models;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Utilities;

namespace CloudCanvas.Web.Controllers
{
    [Route("[controller]")]
    public class GalleryController(ILogger<GalleryController> logger, BlobStorageService bservice) : Controller
    {
        private readonly ILogger<GalleryController> _logger = logger;
        private readonly IBlobStorageService _bservice = bservice;
        // Switched from list of urls to list of DTOs, for immediate metadata retrieval
        // With this, I won't have to fetch further metadata
        // -- saves me expensive calls to azure clients later
        public List<BlobMetaDTO> Blobs { get; set; } = new();

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("[GET] Processing request: Getting blob urls from {service}...", nameof(BlobStorageService));
            Blobs = await _bservice.GetBlobsAsync(BlobStorage.Containers.Uploads);
            _logger.LogInformation("[GET] Succesfully obtained blob urls from {service}...", nameof(BlobStorageService));
            return View(nameof(Index), new GalleryViewModel { 
                Blobs = Blobs
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