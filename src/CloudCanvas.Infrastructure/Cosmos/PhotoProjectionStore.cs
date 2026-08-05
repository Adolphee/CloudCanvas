using CloudCanvas.Application.Abstractions.Projection;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Common.Mapping;
using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Infrastructure.Common;
using CloudCanvas.Infrastructure.Exceptions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
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
            //photo.Id ??= Guid.NewGuid().ToString();
            return await GetContainer(_containerName).CreateItemAsync(photo, new PartitionKey(photo.UserId), default, cancellation);
        }

        public async Task<bool> ReplaceProjectionAsync(PhotoDTO photo, CancellationToken cancellation = default)
        {
            if (photo.Id is null || photo.UserId is null) 
                throw new ProjectionException(message: "both {PhotoId, PhotoUserId} are required.");
            if(await ExistsAsync(new ProjectionKey(photo.Id, photo.UserId), cancellation))
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

        public async Task<bool> DeleteAsync(PhotoDTO meta, bool softDelete = true, CancellationToken cancellationToken = default)
        {
            var container = GetContainer(_containerName);
            var result = await container.DeleteItemAsync<PhotoDTO>(meta.Id, new PartitionKey(meta.UserId), cancellationToken: cancellationToken);
            return result == null;
        }

        public async Task<PhotoDTO?> SingleAsync(ProjectionKey key, CancellationToken cancellation = default)
        {
            var container = GetContainer(_containerName);
            try
            {
                var photo = await container.ReadItemAsync<PhotoDTO>(key.Id, key.AsPartitionKey(), default, cancellation);
                return photo.Resource;
            }
            catch (CosmosException e)
            {
                throw new ProjectionNotFoundException($"Couldn't find requested projection (id={key.Id}.", e)
                {
                    ContainerName = _containerName,
                    DocumentId = key.Id,
                    UserId = key.UserId,
                };
            }
        }

        public async Task<bool> ExistsAsync(ProjectionKey key, CancellationToken cancellationToken = default)
        {
            try
            {
                await SingleAsync(key, cancellationToken);
            }
            catch (Exception e) when (e is CosmosException ||  e is ProjectionNotFoundException)
            {
                _logger.LogTrace(e, "Photo projection not found: {PhotoId}.", key.Id);
                return false; // item doesn't exist if we get to this point
            }
            return true;
        }

        public async Task<PhotoDTO> PatchAsync(ProjectionKey key, IDictionary<string, object> ops, CancellationToken cancellationToken = default)
        {
            var patches = ops.Select(p => PatchOperation.Set(p.Key, p.Value)).ToList();
            _container = GetContainer(_containerName);
            var res = await _container.PatchItemAsync<PhotoDTO>(key.Id, key.AsPartitionKey(), patchOperations: patches, cancellationToken: cancellationToken);
            return res.Resource;
        }

        public async Task<List<PhotoDTO>> GetAllFilteredAsync(Expression<Func<PhotoDTO, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            var con = GetContainer(_containerName);
            var res = new List<PhotoDTO>();
            using var queryable = con.GetItemQueryIterator<PhotoDTO>();
            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellationToken);
                var availableItems = feedResponse.Where(i => i.TimeStamps.DeletedOn <= DateTimeOffset.MinValue);
                if (filter != null)
                {
                    availableItems = availableItems.AsQueryable().Where(filter);
                }
                res.AddRange(availableItems);
            }
            return [.. res];
        }
    }
}
