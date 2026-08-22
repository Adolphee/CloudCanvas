using CloudCanvas.Application.Abstractions.Projection;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Infrastructure.Common;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
namespace CloudCanvas.Infrastructure.Cosmos
{
    public class PhotoProjectionStore(CosmosClient client, IConfiguration config, ILogger<PhotoProjectionStore> logger) : ProjectionStoreBase<PhotoDTO>(client, config, logger), IPhotoProjectionStore
    {
        private Container _container = default!;
        private const string _containerName = Projection.Containers.UserPhotos;

        public async override Task<PhotoDTO> CreateProjectionAsync(PhotoDTO photo, CancellationToken cancellation = default)
        {
            if (photo.UserId is null) 
                throw new ArgumentNullException(nameof(photo), message: "PhotoUserId is required.");
            _container ??= await GetContainerAsync(_containerName, cancellation);
            var res = await _container.CreateItemAsync(photo, new PartitionKey(photo.UserId), default, cancellation);
            return res.Resource;
        }

        public async override Task<bool> ReplaceProjectionAsync(PhotoDTO photo, CancellationToken cancellation = default)
        {
            if (photo.Id is null || photo.UserId is null) 
                throw new ProjectionException(message: "both {PhotoId, PhotoUserId} are required.");
            if(await ExistsAsync(new ProjectionKey(photo.Id, photo.UserId), cancellation))
            {
                _container ??= await GetContainerAsync(_containerName, cancellation);
                var res = await _container.ReplaceItemAsync(photo, photo.Id, new PartitionKey(photo.UserId), default, cancellation);
                return res.StatusCode == System.Net.HttpStatusCode.OK;
            }
            return false;
        }

        public async override Task<List<PhotoDTO>> GetAllAsync(CancellationToken cancellation = default)
        {
            var con = await GetContainerAsync(_containerName, cancellation);
            var res = new List<PhotoDTO>(); 
            using var queryable = con.GetItemQueryIterator<PhotoDTO>();
            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellation);
                var availableItems = feedResponse.Where(i => i.TimeStamps.DeletedOn <= DateTimeOffset.MinValue).OrderByDescending(i => i.TimeStamps.CreatedOn);
                res.AddRange(availableItems);
            }
            return res;
        }

        public async override Task<List<PhotoDTO>> GetByUserIdAsync(string userId, CancellationToken cancellation = default)
        {
            var con = await GetContainerAsync(_containerName, cancellation);
            var res = new List<PhotoDTO>();
            using var queryable = con.GetItemQueryIterator<PhotoDTO>();

            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellation);
                var availableItems = feedResponse.Where(i => i.UserId == userId && (i.TimeStamps.DeletedOn <= DateTimeOffset.MinValue)).OrderByDescending(i => i.TimeStamps.CreatedOn);
                res.AddRange(availableItems);
            }
            return res;
        }

        public async override Task<bool> DeleteAsync(PhotoDTO meta, bool softDelete = true, CancellationToken cancellation = default)
        {
            _container ??= await GetContainerAsync(_containerName, cancellation);
            var result = await _container.DeleteItemAsync<PhotoDTO>(meta.Id, new PartitionKey(meta.UserId), cancellationToken: cancellation);
            return result == null;
        }

        public async override Task<PhotoDTO?> SingleAsync(ProjectionKey key, CancellationToken cancellation = default)
        {
            _container ??= await GetContainerAsync(_containerName, cancellation);
            try
            {
                var photo = await _container.ReadItemAsync<PhotoDTO>(key.Id, key.AsPartitionKey(), default, cancellation);
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

        public async override Task<bool> ExistsAsync(ProjectionKey key, CancellationToken cancellation = default)
        {
            try
            {
                await SingleAsync(key, cancellation);
            }
            catch (Exception e) when (e is CosmosException ||  e is ProjectionNotFoundException)
            {
                _logger.LogTrace(e, "Photo projection not found: {PhotoId}.", key.Id);
                return false; // item doesn't exist if we get to this point
            }
            return true;
        }

        public async override Task<PhotoDTO> PatchAsync(ProjectionKey key, IDictionary<string, object> ops, CancellationToken cancellation = default)
        {
            var patches = ops.Select(p => PatchOperation.Set(p.Key, p.Value)).ToList();
            _container = await GetContainerAsync(_containerName, cancellation);
            var res = await _container.PatchItemAsync<PhotoDTO>(key.Id, key.AsPartitionKey(), patchOperations: patches, cancellationToken: cancellation);
            return res.Resource;
        }

        public async override Task<List<PhotoDTO>> GetAllFilteredAsync(Expression<Func<PhotoDTO, bool>> filter, CancellationToken cancellation = default)
        {
            var con = await GetContainerAsync(_containerName, cancellation);
            var res = new List<PhotoDTO>();
            using var queryable = con.GetItemQueryIterator<PhotoDTO>();
            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellation);
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
