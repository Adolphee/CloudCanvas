using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Commands.SavePhoto;
using CloudCanvas.Domain.Common;
using CloudCanvas.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Commands.UpdatePhoto
{
    public sealed record UpdatePhotoCommand: CreatePhotoCommand
    {
    }
}
