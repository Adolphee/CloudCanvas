using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Domain.Common.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotos
{
    public record GetAllPhotosQuery : IRequest<GetAllPhotosResult>
    {
        public string? UserId { get; set; }
        public string? ContainerName { get; set; } = CloudCosmos.Containers.UserPhotos;
        public PostClassification Type { get; set; } = PostClassification.Photo;
    }

}
