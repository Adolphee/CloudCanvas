using Microsoft.AspNetCore.Mvc;
using CloudCanvas.Shared.Services;
using CloudCanvas.Web.Models;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CloudCanvas.Web.Migrations;
using CloudCanvas.Web.Data;
using CloudCanvas.Web.Utilities;

namespace CloudCanvas.Web.Controllers
{
    [Route("[controller]")]
    public class GalleryController(ILogger<GalleryController> logger, CosmosClientWrapper cosmos_wrapper, UserManager<ApplicationUser> user_mgr) : Controller
    {
        private readonly ILogger<GalleryController> _logger = logger;
        private readonly ICosmosClientWrapper _cosmos = cosmos_wrapper;
        private readonly UserManager<ApplicationUser> _userMgr = user_mgr;

        [HttpGet, Authorize]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("[GET] Getting gallery items from {service}...", nameof(BlobStorageService));
            var blobs = await _cosmos.ListBlobsAsync<BlobMetaDTO>(CloudCosmos.Containers.BlobMeta);
            _logger.LogInformation("[GET] Succesfully obtained gallery items from {service}...", nameof(BlobStorageService));
            var user = await _userMgr.GetUserAsync(User);
            return View("GalleryItemsList", new GalleryViewModel
            {
                Blobs = blobs,
                Photos = blobs.Select(x => x.Convert(user!)).ToList(),
                curentUser = user!
            });
        }
    }
}