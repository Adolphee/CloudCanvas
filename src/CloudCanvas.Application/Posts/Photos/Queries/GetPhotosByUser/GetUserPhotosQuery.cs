using CloudCanvas.Application.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetUserPhotosQuery
    {
        public string UserId { get; set; }
        public string ContainerName { get; set; }

        public GetUserPhotosQuery(string id, string container = CloudCosmos.Containers.BlobMeta)
        {
            UserId = id;
            ContainerName = container;
        }
    }
}
