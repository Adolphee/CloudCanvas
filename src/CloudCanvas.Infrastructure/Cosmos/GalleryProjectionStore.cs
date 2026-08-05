using CloudCanvas.Application.Abstractions.Projection;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.Galleries;
using CloudCanvas.Application.Posts.Galleries.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace CloudCanvas.Infrastructure.Cosmos
{
    public class GalleryProjectionStore : IGalleryProjectionStore
    {
        private Container _container = null!; // The Cosmos DB container for gallery projections
        private readonly CosmosClient _client;
        private readonly ILogger<GalleryProjectionStore> _logger;

        public GalleryProjectionStore(CosmosClient client, ILogger<GalleryProjectionStore> logger)
        {
            _client = client;
            _logger = logger;
        }

        private async Task<Container> GetOrCreateContainerAsync(string? containerId = default)
        {
            containerId ??= Projection.Containers.Galleries;
            if (_container != null && _container.Id == containerId) return _container;

            Database database = _client.GetDatabase(Projection.Sql);
            var res = await database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = containerId,
                PartitionKeyPath = "/userId"
            });

            return res.Container;
        }

        public async Task<List<GalleryDTO>> GetAllFilteredAsync(Expression<Func<GalleryDTO, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            _container = await GetOrCreateContainerAsync();
            var queryRes = _container.GetItemLinqQueryable<GalleryDTO>();
            IQueryable<GalleryDTO> filteredResult = queryRes.Where(g => g.TimeStamps.DeletedOn == DateTimeOffset.MinValue);
            if (filter != null)
            {
                filteredResult = filteredResult.Where(filter);
            }
            var galleries = new List<GalleryDTO>();
            var fi = filteredResult.ToFeedIterator();
            while (fi.HasMoreResults)
            {
                var response = await fi.ReadNextAsync(cancellationToken);
                galleries.AddRange(response.ToList());
            }
            ;
            return galleries;
        }

        public async Task<GalleryDTO> CreateProjectionAsync(GalleryDTO gallery, CancellationToken cancellationToken = default)
        {
            _container = await GetOrCreateContainerAsync();
            var response = await _container.CreateItemAsync(gallery, new PartitionKey(gallery.UserId), cancellationToken: cancellationToken);
            return response.Resource;
        }

        public async Task<bool> DeleteAsync(GalleryDTO gallery, bool softDelete = true, CancellationToken cancellationToken = default)
        {
            _container = await GetOrCreateContainerAsync();
            var key = new ProjectionKey(gallery.Id!, gallery.UserId!);
            if (await ExistsAsync(key, cancellationToken))
            {
                if (softDelete)
                {
                    gallery.TimeStamps.DeletedOn = DateTimeOffset.UtcNow;
                    await _container.ReplaceItemAsync(gallery, gallery.Id!, new PartitionKey(gallery.UserId!), cancellationToken: cancellationToken);
                }
                else
                {
                    await _container.DeleteItemAsync<GalleryDTO>(gallery.Id!, new PartitionKey(gallery.UserId!), cancellationToken: cancellationToken);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ExistsAsync(ProjectionKey key, CancellationToken cancellationToken = default)
        {
            _container = await GetOrCreateContainerAsync();
            return await _container.ReadItemAsync<GalleryDTO>(key.Id, new PartitionKey(key.UserId), cancellationToken: cancellationToken) != null;
        }

        public async Task<List<GalleryDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllFilteredAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<GalleryDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            _container = await GetOrCreateContainerAsync();
            return await GetAllFilteredAsync(g => g.UserId == userId, cancellationToken);
        }

        public async Task<GalleryDTO> PatchAsync(ProjectionKey key, IDictionary<string, object> ops, CancellationToken cancellationToken = default)
        {
            _container = await GetOrCreateContainerAsync();
            if(!await ExistsAsync(key, cancellationToken))
            {
                throw new InvalidOperationException($"Gallery with Id {key.Id} and UserId {key.UserId} does not exist.");
            }
            return await _container.PatchItemAsync<GalleryDTO>(key.Id, new PartitionKey(key.UserId), ops.Select(kv => PatchOperation.Replace($"/{kv.Key}", kv.Value)).ToList(), cancellationToken: cancellationToken);
        }

        public async Task<bool> ReplaceProjectionAsync(GalleryDTO gallery, CancellationToken cancellation = default)
        {
            _container = await GetOrCreateContainerAsync();
            if (!await ExistsAsync(new(gallery.Id, gallery.UserId), cancellation))
            {
                throw new InvalidOperationException($"Gallery with Id {gallery.Id} and UserId {gallery.UserId} does not exist.");
            }
            await _container.ReplaceItemAsync(gallery, gallery.Id!, new PartitionKey(gallery.UserId), cancellationToken: cancellation);
            return true;
        }

        public async Task<GalleryDTO?> SingleAsync(ProjectionKey key, CancellationToken cancellationToken = default)
        {
            _container = await GetOrCreateContainerAsync();
            return await _container.ReadItemAsync<GalleryDTO>(key.Id, new PartitionKey(key.UserId), cancellationToken: cancellationToken);
        }
    }
}
