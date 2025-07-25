using Microsoft.AspNetCore.Mvc;
using CloudCanvas.Web.Models;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Utilities;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Shared.DTOs;

namespace CloudCanvas.Web.Controllers
{
    public class UploadController(BlobStorageService service, ILogger<UploadController> logger, CosmosClientWrapper cosmos) : Controller
    {
        private readonly ILogger<UploadController> _logger = logger;
        private readonly IBlobStorageService _service = service;
        private readonly ICosmosClientWrapper _cosmos = cosmos;

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(UploadViewModel userUpload)
        {
            var file = userUpload.File;
            // 1. validate the file
            // TODO: implement full file inspection, file type, extension, ...
            Validate.Object(file); 
            // 2. use the BlobSorageService to persist
            var meta = await _service.UploadAsync(file.OpenReadStream(), file.FileName, BlobStorage.Containers.Uploads);
            // I am already saving what I can to CosmosDB, so that the frontend can access it already
            // Functions will take care of any further updates, while for now the user gets the bare minimum
            meta.Name = userUpload.File.FileName;
            meta.ProcessingStage = (int) BlobProcessingStage.UploadSuccessful;
            await _cosmos.SaveMetadataAsync(meta, CloudCosmos.Containers.BlobMeta);
            // 3. redirect to gallery
            return RedirectToAction("Index", "Gallery");
        }
    }
}