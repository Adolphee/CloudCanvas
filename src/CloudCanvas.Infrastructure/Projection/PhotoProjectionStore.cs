using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace CloudCanvas.Infrastructure.Projection
{
    public class PhotoProjectionStore : ProjectionStoreBase<PhotoDTO>, IPhotoProjectionStore
    {
        public PhotoProjectionStore(CosmosClient client, IConfiguration config, ILogger<PhotoProjectionStore> logger) : base(client, config, logger)
        {
            _containerName ??= InferContainerNameFromType();
        }
    }
}
