using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CloudCanvas.Models.ViewModels
{
    public class GalleryViewModel
    {
        [BindProperty]
        public required List<string> ImageLinks { get; set; }
    }
}