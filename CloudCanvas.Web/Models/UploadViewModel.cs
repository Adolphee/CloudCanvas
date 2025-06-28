using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CloudCanvas.Web.Models
{
    public class UploadViewModel
    {
        [Required]
        [BindProperty]
        public required IFormFile File { get; set; }
    }
}