using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Infrastructure.Exceptions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
namespace CloudCanvas.Infrastructure.Cosmos
{
    public class PhotoProjectionStore(CosmosClient client, ILogger<PhotoProjectionStore> logger) : IPhotoProjectionStore
    {
        private Container _container = default!;
        private readonly CosmosClient _client = client;
        private const string _containerName = Projection.Containers.UserPhotos;
        private readonly ILogger<PhotoProjectionStore> _logger = logger;

        public async Task<PhotoDTO> CreateProjectionAsync(PhotoDTO photo, CancellationToken cancellation = default)
        {
            if (photo.UserId is null) 
                throw new ArgumentNullException(nameof(photo), message: "PhotoUserId is required.");
            photo.Id ??= Guid.NewGuid().ToString();
            return await GetContainer(_containerName).CreateItemAsync(photo, new PartitionKey(photo.UserId), default, cancellation);
        }

        public async Task<bool> ReplaceProjectionAsync(PhotoDTO photo, CancellationToken cancellation = default)
        {
            if (photo.Id is null || photo.UserId is null) 
                throw new ProjectionException(message: "both {PhotoId, PhotoUserId} are required.");
            if(await ExistsAsync(photo.Id, photo.UserId, cancellation))
            {
                var res = await GetContainer(_containerName).ReplaceItemAsync(photo, photo.Id, new PartitionKey(photo.UserId), default, cancellation);
                return res.StatusCode == System.Net.HttpStatusCode.OK;
            }
            return false;
        }

        private Container GetContainer(string? containerId = default)
        {
            containerId ??= _containerName;
            if (_container != null && _container.Id == containerId) return _container;
            // At this point, either _container is null or it's not the one we want, so we get it from the client
            try
            {
                _container = _client.GetContainer(Projection.Sql, containerId);
            }
            catch (Exception e) { 
                throw new CosmosContainerNotFoundException($"Container Not Found: '{containerId}'", e)
                {
                    ContainerName = containerId,
                    DatabaseName = Projection.Sql
                };
            }
            return _container;
        }

        public async Task<List<PhotoDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var con = GetContainer(_containerName);
            var res = new List<PhotoDTO>(); 
            using var queryable = con.GetItemQueryIterator<PhotoDTO>();
            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellationToken);
                var availableItems = feedResponse.Where(i => i.TimeStamps.DeletedOn <= DateTimeOffset.MinValue).OrderByDescending(i => i.TimeStamps.CreatedOn);
                res.AddRange(availableItems);
            }
            return [..res];
        }

        public async Task<List<PhotoDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var con = GetContainer(_containerName);
            var res = new List<PhotoDTO>();
            using var queryable = con.GetItemQueryIterator<PhotoDTO>();

            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellationToken);
                var availableItems = feedResponse.Where(i => i.UserId == userId && (i.TimeStamps.DeletedOn <= DateTimeOffset.MinValue)).OrderByDescending(i => i.TimeStamps.CreatedOn);
                res.AddRange(availableItems);
            }
            return res;
        }

        public async Task<bool> DeleteAsync(PhotoDTO meta, CancellationToken cancellationToken = default)
        {
            var container = GetContainer(_containerName);
            var result = await container.DeleteItemAsync<PhotoDTO>(meta.Id, new PartitionKey(meta.UserId), cancellationToken: cancellationToken);
            return result == null;
        }

        public async Task<PhotoDTO> SingleAsync(string Id, string userId, CancellationToken cancellationToken = default)
        {
            var container = GetContainer(_containerName);
            var photo = await container.ReadItemAsync<PhotoDTO>(Id, new PartitionKey(userId), cancellationToken: cancellationToken);
            return photo != null ? photo.Resource : throw new CosmosDocumentNotFoundException
            {
                ContainerName = _containerName,
                DocumentId = Id,
                UserId = userId
            };
        }

        public async Task<bool> ExistsAsync(string id, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                await SingleAsync(id, userId, cancellationToken);
            }
            catch (Exception e) when (e is CosmosException ||  e is CosmosDocumentNotFoundException)
            {
                _logger.LogTrace(e, "Photo projection not found: {PhotoId}.", id);
                return false; // item doesn't exist if we get to this point
            }
            return true;
        }

        public async Task<PhotoDTO> PatchAsync(string identifier, string userId, IDictionary<string, object> ops, CancellationToken cancellationToken = default)
        {
            var patches = ops.Select(p => PatchOperation.Set(p.Key, p.Value)).ToList();
            _container = GetContainer(_containerName);
            var res = await _container.PatchItemAsync<PhotoDTO>(id: identifier, partitionKey: new PartitionKey(userId), patchOperations: patches, cancellationToken: cancellationToken);
            return res.Resource;
        }
    }
}
