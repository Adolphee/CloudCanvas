using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetUserPhotosQuery: IRequest<GetUserPhotosResult>
    {
        public string UserId { get; set; }
        public string ContainerName { get; set; }

        public GetUserPhotosQuery(string id, string container = CloudCosmos.Containers.UserPhotos)
        {
            UserId = id;
            ContainerName = container;
        }
    }
}
