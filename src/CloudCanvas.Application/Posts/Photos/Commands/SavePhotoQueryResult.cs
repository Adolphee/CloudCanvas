using CloudCanvas.Application.Posts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Commands
{
    public sealed record SavePhotoQueryResult
    {
        public bool IsSuccessFull { get; set; } = false;
        public PhotoDTO? Photo { get; set; } = default!;
    }
}
