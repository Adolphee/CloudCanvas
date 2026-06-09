using Microsoft.AspNetCore.Mvc;
using CloudCanvas.Shared.Services;
using CloudCanvas.Web.Models;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace CloudCanvas.Web.Controllers
{
    [Route("[controller]")]
    public class GalleryController(ILogger<GalleryController> logger, CosmosClientWrapper cosmos_wrapper) : Controller
    {
        private readonly ILogger<GalleryController> _logger = logger;
        private readonly ICosmosClientWrapper _cosmos = cosmos_wrapper;

        [HttpGet, Authorize]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("[GET] Getting gallery items from {service}...", nameof(BlobStorageService));
            var blobs = await _cosmos.ListBlobsAsync<BlobMetaDTO>(CloudCosmos.Containers.BlobMeta);
            _logger.LogInformation("[GET] Succesfully obtained gallery items from {service}...", nameof(BlobStorageService));
            return View("GalleryItemsList", new GalleryViewModel { 
                Blobs = blobs
            });
        }
    }
}