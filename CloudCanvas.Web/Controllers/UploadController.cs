using Microsoft.AspNetCore.Mvc;
using CloudCanvas.Web.Models;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Constants;

namespace CloudCanvas.Web.Controllers
{
    public class UploadController : Controller
    {
        private readonly BlobStorageService _service;
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
            // TODO: 1. validate the file
            // 2. use the BlobSorageService to persist
            await _service.UploadAsync(BlobStorage.Containers.Uploads, file.OpenReadStream(), file.FileName);
            // 3. redirect to gallery
            return RedirectToAction("Index", "Gallery");
        }
    }
}