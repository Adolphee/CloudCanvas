using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetAllPostsQuery
    {
        public string? UserId { get; set; }
        public string? ContainerName { get; set; } = CloudCosmos.Containers.UserPhotos;
        public PostClassification Type { get; set; } = PostClassification.Photo;
    }

}
