using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using CloudCanvas.Web.Data;
using CloudCanvas.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CloudCanvas.Web.Controllers
{
    public class UploadController(BlobStorageService service, ILogger<UploadController> logger, CosmosClientWrapper cosmos, UserManager<ApplicationUser> userMgr) : Controller
    {
        private readonly ILogger<UploadController> _logger = logger;
        private readonly IBlobStorageService _service = service;
        private readonly ICosmosClientWrapper _cosmos = cosmos;
        private readonly UserManager<ApplicationUser> _userMgr = userMgr;

        [HttpGet, Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> UploadAsync(UploadViewModel userUpload)
        {
            var currentUser = await _userMgr.GetUserAsync(User);
            var file = userUpload.File;
            var uploadsContainer = BlobStorage.Containers.Uploads;
            var properties = BlobStorageService.SetOriginalMetadata(file.FileName, currentUser!.Id);

            _logger.LogInformation("Received file '{fileName}', uploading to '{containerName}'", file.FileName, uploadsContainer);
            
            var meta =await _service.UploadAsync(file, properties); 
            if (meta != null) { meta.UserId = currentUser!.Id; meta.UploadedBy = currentUser.Id; }
            await _cosmos.SaveMetadataAsync(meta!, CloudCosmos.Containers.BlobMeta); // instant access for frontend
            
            return RedirectToAction("Index", "Gallery");
        }
    }

}