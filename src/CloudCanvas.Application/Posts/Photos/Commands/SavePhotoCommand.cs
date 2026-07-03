using CloudCanvas.Application.Posts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Commands
{
    public sealed record SavePhotoCommand
    {
        public string UserId { get; set; } = default!;
        public PhotoDTO Photo { get; set; } = default!;
    }
}
