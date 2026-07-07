using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetUserPhotosResult : GetAllPhotosResult
    {
        public GetUserPhotosResult(List<PhotoDTO> posts) : base(posts) { }
    }
}
