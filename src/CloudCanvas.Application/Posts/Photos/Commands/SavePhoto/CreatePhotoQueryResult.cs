using CloudCanvas.Application.Posts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Commands.SavePhoto
{
    public sealed record CreatePhotoQueryResult
    {
        public bool IsSuccessFull { get; set; } = false;
        public PhotoDTO? Photo { get; set; } = default!;
    }
}
