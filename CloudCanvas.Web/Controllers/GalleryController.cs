using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CloudCanvas.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared;

namespace CloudCanvas.Web.Controllers
{
    [Route("[controller]")]
    public class GalleryController : Controller
    {
        private readonly ILogger<GalleryController> _logger;
        private readonly BlobStorageService _service;
        public List<string> ImageLinks { get; set; } = new();

        public GalleryController(ILogger<GalleryController> logger, BlobStorageService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ImageLinks = await _service.GetUrlsAsync(BlobStorage.Containers.Uploads);
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