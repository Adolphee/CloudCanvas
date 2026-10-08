using System.Linq.Expressions;
using CloudCanvas.Application.Abstractions.Projection;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.Comments;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Galleries;
using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Infrastructure.Common;
using CloudCanvas.Infrastructure.Exceptions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Infrastructure.Projection
{
    public abstract class ProjectionStoreBase<T>(CosmosClient client, IConfiguration config, ILogger logger) : IProjectionStoreBase<T> where T : PostDTO
    {
        protected readonly CosmosClient _client = client;
        protected readonly IConfiguration _config = config;
        protected readonly ILogger _logger = logger;
        protected string _containerName { get; init; } = null!;
        protected Container _container { get; private set; } = null!; 

        private async Task<Container> EnsureContainerExistsAsync(string database, string containerId, CancellationToken cancellation = default)
        {
            var result = await _client.CreateDatabaseIfNotExistsAsync(database, cancellationToken: cancellation);
            var res = await result.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = containerId,
                PartitionKeyPath = "/userId"
            }, cancellationToken: cancellation);
            return res.Container;
        }

        protected async Task<Container> GetContainerAsync(string containerId, CancellationToken cancellation = default)
        {
            string? databaseName = _config[AppSettings.ProjectionDbName] ?? Application.Common.Constants.Projection.Sql;
            try
            {
                return await EnsureContainerExistsAsync(databaseName, containerId, cancellation);
            }
            catch (Exception e) {
                throw new CosmosContainerNotFoundException($"Failed to ensure the existence of {containerId} in {databaseName}", e)
                {
                    ContainerName = containerId,
                    DatabaseName = databaseName
                };
            }
        }

        public async Task<T> CreateProjectionAsync(T photo, CancellationToken cancellation = default)
        {
            if (photo.UserId is null) throw new ArgumentNullException(nameof(photo), message: "Value for Photo.UserId is required.");
            _container ??= await GetContainerAsync(_containerName, cancellation);
            var res = await _container.UpsertItemAsync(photo, new PartitionKey(photo.UserId), default, cancellation);
            return res.Resource;
        }
        
        public async Task<bool> DeleteAsync(T meta, bool softDelete = true, CancellationToken cancellation = default)
        {
            _container ??= await GetContainerAsync(_containerName, cancellation);
                if (softDelete)
                {
                    meta.TimeStamps.DeletedOn = DateTimeOffset.UtcNow;
                    var ops = new Dictionary<string, object> { ["/timestamps/deletedOn"] = DateTimeOffset.UtcNow };
                    var res = await PatchAsync(new ProjectionKey(meta.Id, meta.UserId!), ops, cancellation);
                    return res.TimeStamps.DeletedOn != DateTimeOffset.MinValue;
                }
                try
                {
                    var res = await _container.DeleteItemAsync<T>(meta.Id!, new PartitionKey(meta.UserId!), cancellationToken: cancellation);
                    return res.StatusCode == System.Net.HttpStatusCode.NoContent;
                } catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ProjectionNotFoundException($"Projection not found in container {_containerName} for Id={meta.Id} and UserId={meta.UserId}.", e)
                    {
                        ContainerName = _containerName,
                        DocumentId = meta.Id,
                        UserId = meta.UserId
                    };
                }
        }

        protected string InferContainerNameFromType()
        {
            var typeName = typeof(T).Name;
            return typeName switch
            {
                nameof(PhotoDTO) => Application.Common.Constants.Projection.Containers.UserPhotos,
                nameof(GalleryDTO) => Application.Common.Constants.Projection.Containers.Galleries,
                nameof(CommentDTO) => Application.Common.Constants.Projection.Containers.Comments,
                _ => throw new NotImplementedException($"No container mapping defined for type {typeName}.")
            };
        }

        public async Task<T> PatchAsync(ProjectionKey key, IDictionary<string, object> ops, CancellationToken cancellation = default)
        {
            var patches = ops.Select(p => PatchOperation.Set(p.Key, p.Value)).ToList();
            _container ??= await GetContainerAsync(_containerName, cancellation);
            try
            {
                return await _container.PatchItemAsync<T>(key.Id, key.AsPartitionKey(), patchOperations: patches, cancellationToken: cancellation)
                    .ContinueWith(t => t.Result.Resource, cancellation);
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ProjectionNotFoundException($"Projection not found in container {_containerName} for Id={key.Id} and UserId={key.UserId}.", e)
                {
                    ContainerName = _containerName,
                    DocumentId = key.Id,
                    UserId = key.UserId
                };
            }
        }

        public async Task<T?> SingleAsync(ProjectionKey key, CancellationToken cancellation = default)
        {
        _container ??= await GetContainerAsync(_containerName, cancellation);
            try
            {
                var photo = await _container.ReadItemAsync<T>(key.Id, key.AsPartitionKey(), default, cancellation);
                return photo.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ProjectionNotFoundException($"Couldn't find requested projection (id={key.Id}).", e)
                {
                    ContainerName = _containerName,
                    DocumentId = key.Id,
                    UserId = key.UserId,
                };
            }
        }
        
        public async Task<List<T>> GetByUserIdAsync(string userId, CancellationToken cancellation = default)
        {
            _container ??= await GetContainerAsync(_containerName, cancellation);
            var res = new List<T>();
            using var queryable = _container.GetItemQueryIterator<T>();

            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellation);
                var availableItems = feedResponse.Where(i => i.UserId == userId && (i.TimeStamps.DeletedOn <= DateTimeOffset.MinValue)).OrderByDescending(i => i.TimeStamps.CreatedOn);
                res.AddRange(availableItems);
            }
            return [.. res];
        }

        public async Task<List<T>> GetAllAsync(CancellationToken cancellation = default)
        {
            _container ??= await GetContainerAsync(_containerName, cancellation);
            var res = new List<T>();
            using var queryable = _container.GetItemQueryIterator<T>();
            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellation);
                var availableItems = feedResponse.Where(i => i.TimeStamps.DeletedOn <= DateTimeOffset.MinValue).OrderByDescending(i => i.TimeStamps.CreatedOn);
                res.AddRange(availableItems);
            }
            return [.. res];
        }

        public async Task<bool> ReplaceProjectionAsync(T photo, CancellationToken cancellation = default)
        {
            if (photo.Id is null || photo.UserId is null)
                throw new ProjectionException(message: "Both {PhotoId, Photo.UserId} are required.");
            try
            {
                var _container = await GetContainerAsync(_containerName, cancellation);
                var res = await _container.ReplaceItemAsync(photo, photo.Id, new PartitionKey(photo.UserId), default, cancellation);
                return res.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ProjectionNotFoundException($"Projection not found for PhotoId={photo.Id} and UserId={photo.UserId}.", e)
                {
                    ContainerName = _containerName,
                    DocumentId = photo.Id,
                    UserId = photo.UserId
                };
            } finally
            {
                _logger.LogTrace("ReplaceProjectionAsync completed for PhotoId={PhotoId} and UserId={UserId}.", photo.Id, photo.UserId);
            }
        }

        public async Task<List<T>> GetAllFilteredAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation = default)
        {
            _container ??= await GetContainerAsync(_containerName, cancellation);
            var res = new List<T>();
            using var queryable = _container.GetItemQueryIterator<T>();
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