using CloudCanvas.Application.Posts.Galleries;
using CloudCanvas.Application.Posts.Galleries.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Infrastructure.Projection
{
    public class GalleryProjectionStore: ProjectionStoreBase<GalleryDTO>, IGalleryProjectionStore
    {
        public GalleryProjectionStore(CosmosClient client, IConfiguration config, ILogger<GalleryProjectionStore> logger) : base(client, config, logger)
        {
            _containerName ??= InferContainerNameFromType();
        }
    }
}
