using CloudCanvas.Shared.Constants;
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
            _logger.LogInformation("Received file '{fileName}', uploading to '{containerName}'", file.FileName, uploadsContainer);
            // 1. validate the file
            // TODO: implement full file inspection, file type, extension, ...
            try
            {
                Validate.Object(file);
            } catch(InvalidArgumentException e)
            {
                _logger.LogError(e, "Encountered invalid/corrupted file '{filename}' while validating upload to '{containerName}'", file.FileName, uploadsContainer);
                return View(new ErrorViewModel { 
                    Message = "ERROR - Darn it! This file didn't make the cut. Please consider another one.",
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
                });
            }
            // 2. use the BlobSorageService to persist
            var meta = await _service.UploadAsync(file.OpenReadStream(), file.FileName, uploadsContainer);
            // I am already saving what I can to CosmosDB, so that the frontend can access it immediately
            // Functions will take care of any further updates, while for now the end user gets the latest relavant data for them
            if (meta != null) meta.UserId = currentUser!.Id;
            await _cosmos.SaveMetadataAsync(meta!, CloudCosmos.Containers.BlobMeta);
            // 3. redirect to gallery
            return RedirectToAction("Index", "Gallery");
        }
    }
}