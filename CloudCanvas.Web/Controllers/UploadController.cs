using Microsoft.AspNetCore.Mvc;
using CloudCanvas.Web.Models;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Utilities;
using CloudCanvas.Shared.Interfaces;

namespace CloudCanvas.Web.Controllers
{
    public class UploadController : Controller
    {
        private readonly IBlobStorageService _service;
        private readonly ILogger<UploadController> _logger;
        public UploadController(BlobStorageService service, ILogger<UploadController> logger)
        {
            _service = service;
            _logger = logger;
        }

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
            await _service.UploadAsync(file.OpenReadStream(), file.FileName, BlobStorage.Containers.Uploads);
            // 3. redirect to gallery
            return RedirectToAction("Index", "Gallery");
        }
    }
}