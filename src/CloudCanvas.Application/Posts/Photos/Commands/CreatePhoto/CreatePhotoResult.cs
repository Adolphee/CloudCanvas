using CloudCanvas.Application.Posts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto
{
    public sealed record CreatePhotoResult
    {
        public bool Success { get; set; } = false;
        public PhotoDTO? Photo { get; set; } = default!;
    }
}
