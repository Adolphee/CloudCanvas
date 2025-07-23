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
        private readonly ICosmosClientWrapper _cosmos;
        public List<BlobMetaDTO> blobsMetadataList { get; set; } = new();

        public GalleryController(ILogger<GalleryController> logger, CosmosClientWrapper cosmos)
        {
            _logger = logger;
            _cosmos = cosmos;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            blobsMetadataList = await _cosmos.ListBlobsAsync(CloudCosmos.Containers.BlobMeta);
            return View(nameof(Index), new GalleryViewModel { Images = blobsMetadataList });
        }

        [HttpGet("gallery/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}