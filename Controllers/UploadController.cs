using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudCanvas.Interfaces;
using CloudCanvas.Models;
using CloudCanvas.Models.ViewModels;
using CloudCanvas.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudCanvas.Controllers
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
            // TODO: 1. validate the file
            // 2. use the BlobSorageService to persist
            await _service.UploadAsync(userUpload.File);
            // 3. redirect to gallery
            return RedirectToAction("Index", "Gallery");
        }
    }
}