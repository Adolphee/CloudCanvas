using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Infrastructure.DTOs;
using CloudCanvas.Infrastructure.Exceptions;
using CloudCanvas.Infrastructure.Identity;
using Mapster;
using Microsoft.Azure.Cosmos;
using CCContainers = CloudCanvas.Application.Common.Constants.Projection.Containers;
namespace CloudCanvas.Infrastructure.Cosmos
{
    public class PhotoProjectionStore(CosmosClient client) : IPhotoProjectionStore
    {
        private readonly CosmosClient _client = client;
        private const string _containerName = CCContainers.UserPhotos;
        private Container? _container;

        public async Task<PhotoDTO> SaveProjectionAsync(PhotoDTO photo, string containerName = _containerName, bool overWrite = true, CancellationToken cancellationToken = default)
        {
            var container = _client.GetContainer(Projection.Sql, containerName);
            ItemResponse<PhotoDTO> result;
            if (photo.Id == null) photo.Id = Guid.NewGuid().ToString();
            var partitionKey = new PartitionKey(photo.UserId);
            if (overWrite) result = await container.ReplaceItemAsync(photo, photo.Id, partitionKey, cancellationToken: cancellationToken);
            else result = await container.CreateItemAsync(photo, partitionKey, cancellationToken: cancellationToken);
            return result.Resource;
        }

        private Container GetContainer(string containerId)
        {
            if(_container != null && _container.Id == containerId) return _container;
            // At this point, either _container is null or it's not the one we want, so we get it from the client
            try
            {
                _container = _client.GetContainer(Projection.Sql, containerId);
            }
            catch (Exception e)
            {   // Custom exception wrapped around the original, for more context
                throw new CosmosContainerNotFoundException($"Container Not Found: '{containerId}'", e)
                {
                    ContainerName = containerId,
                    DatabaseName = Projection.Sql
                };
            }
            return _container;
        }

        public async Task<List<PhotoDTO>> GetAllAsync(string containerName, CancellationToken cancellationToken = default)
        {
            var con = GetContainer(containerName);
            var res = new List<PhotoDTO>(); 
            using var queryable = con.GetItemQueryIterator<PhotoDTO>();
            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellationToken);
                var availableItems = feedResponse.Where(i => i.TimeStamps.DeletedOn <= DateTimeOffset.MinValue);
                res.AddRange(availableItems);
            }
            return res.ToList();
        }

        public async Task<List<PhotoDTO>> GetByUserIdAsync(string userId, string containerName, CancellationToken cancellationToken = default)
        {
            var con = GetContainer(containerName);
            var res = new List<PhotoDTO>();
            using var queryable = con.GetItemQueryIterator<PhotoDTO>();

            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellationToken);
                var availableItems = feedResponse.Where(i => i.UserId == userId && (i.TimeStamps.DeletedOn <= DateTimeOffset.MinValue));
                res.AddRange(availableItems.Adapt<List<PhotoDTO>>());
            }
            return res;
        }

        public async Task<bool> DeleteAsync(PhotoDTO meta, string containerName, CancellationToken cancellationToken = default)
        {
            var container = GetContainer(containerName);
            var result = await container.DeleteItemAsync<PhotoDTO>(meta.Id, new PartitionKey(meta.UserId), cancellationToken: cancellationToken);
            return result == null;
        }

        public async Task<PhotoDTO> SingleAsync(string documentId, string userId, string containerName, CancellationToken cancellationToken = default)
        {
            var container = GetContainer(containerName);
            var blob = await container.ReadItemAsync<PhotoDTO>(documentId, new PartitionKey(userId), cancellationToken: cancellationToken);
            if (blob == null) throw new CosmosDocumentNotFoundException
            {
                ContainerName = containerName,
                DocumentId = documentId,
                UserId = userId
            };

            return blob;
        }
        public async Task<bool> ExistsAsync(string containerName, string id, string partitionKey, CancellationToken cancellationToken = default)
        {
            var container = GetContainer(containerName);
            try
            {
                await container.ReadItemAsync<BlobMetadata>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
                return true;
            }
            catch (CosmosException)
            {
                return false;
            }
        }

        public async Task<PhotoDTO> PatchAsync(string identifier, string userId, string containerName, IDictionary<string, object> ops, CancellationToken cancellationToken = default)
        {
            var patches = ops.Select(p => PatchOperation.Set(p.Key, p.Value)).ToList();
            _container = GetContainer(containerName);
            var res = await _container.PatchItemAsync<PhotoDTO>(id: identifier, partitionKey: new PartitionKey(userId), patchOperations: patches, cancellationToken: cancellationToken);
            return res.Resource;
        }
    }
}
