using CloudCanvas.Application.Abstractions.Cosmos;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Infrastructure.DTOs;
using CloudCanvas.Infrastructure.Exceptions;
using Mapster;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CloudCanvas.Infrastructure.Cosmos
{
    public class CosmosClientWrapper<T> : IPostsRepositoryCosmos<T> where T : IPost
    {
        private readonly CosmosClient _client;
        private Container? _container;
        public CosmosClientWrapper(CosmosClient client)
        {
            _client = client;
        }

        public async Task<T> PatchItemAsync(string identifier, string userId, string containerName, IReadOnlyList<PatchOperation> ops, CancellationToken cancellationToken = default)
        {
            _container = GetContainer(containerName);
            var metadata = await _container.PatchItemAsync<T>(id: identifier, partitionKey: new PartitionKey(userId), patchOperations: ops, cancellationToken: cancellationToken);
            return metadata;
        }

        /// <summary>
        /// Asynchronously saves the specified object to the given Cosmos DB container.
        /// </summary>
        /// <remarks>This method performs schema validation on the object using data annotations defined
        /// in the <see cref="MetadataDocumentBase"/> class or its derived types. If validation fails, the errors are
        /// logged, and an <see cref="ArgumentException"/> is thrown. The object is then upserted into the specified
        /// Cosmos DB container.</remarks>
        /// <typeparam name="T">The type of the object to save. Must inherit from <see cref="MetadataDocumentBase"/>.</typeparam>
        /// <param name="metadata">The object to be saved. The object must have valid data annotations and non-empty <see
        /// cref="MetadataDocumentBase.Id"/> and <see cref="MetadataDocumentBase.UserId"/> properties.</param>
        /// <param name="containerName">The name of the Cosmos DB container where the object will be saved.</param>
        /// <param name="documentId">The Id of the Cosmos DB docuement to be updated. If not [null/whitespace/empty], then this becomes an update operation.</param>
        /// <returns>The saved object as returned by Cosmos DB after the upsert operation.</returns>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="metadata"/> is invalid, or if the <see cref="MetadataDocumentBase.Id"/> or <see
        /// cref="MetadataDocumentBase.UserId"/> properties are null, empty, or whitespace.</exception>
        public async Task<T> SaveMetadataAsync(T metadata, string containerName, bool overWrite = true, CancellationToken cancellationToken = default)
        {
            var container = _client.GetContainer(CloudCosmos.Sql, containerName);
            ItemResponse<T> result;
            var partitionKey = new PartitionKey(metadata.UserId);
            if (metadata.Id == null) metadata.Id = Guid.NewGuid().ToString();
            if (overWrite) result = await container.ReplaceItemAsync(metadata, metadata.Id, partitionKey, cancellationToken: cancellationToken);
            else result = await container.CreateItemAsync(metadata, partitionKey, cancellationToken: cancellationToken);
            return result.Resource;
        }
        
        public async Task<PhotoDTO> SavePhotoAsync(PhotoDTO metadata, string containerName, bool overWrite = true, CancellationToken cancellationToken = default)
        {
            var container = _client.GetContainer(CloudCosmos.Sql, containerName);
            ItemResponse<PhotoDTO> result;
            if (metadata.Id == null) metadata.Id = Guid.NewGuid().ToString();
            var partitionKey = new PartitionKey(metadata.Creator!.GetId());
            if (overWrite) result = await container.ReplaceItemAsync(metadata, metadata.Id, partitionKey, cancellationToken: cancellationToken);
            else result = await container.CreateItemAsync(metadata, partitionKey, cancellationToken: cancellationToken);
            return result.Resource;
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

        private Container GetContainer(string containerId)
        {
            if(_container != null && _container.Id == containerId) return _container;
            // At this point, either _container is null or it's not the one we want, so we get it from the client
            try
            {
                _container = _client.GetContainer(CloudCosmos.Sql, containerId);
            }
            catch (Exception e)
            {   // Custom exception wrapped around the original, for more context
                throw new CosmosContainerNotFoundException($"Container Not Found: '{containerId}'", e)
                {
                    ContainerName = containerId,
                    DatabaseName = CloudCosmos.Sql
                };
            }
            return _container;
        }

        public async Task<List<Post>> ListBlobsAsync(string containerName, CancellationToken cancellationToken = default)
        {
            var con = GetContainer(containerName);
            var queryable = con.GetItemLinqQueryable<Post>().Where(x => x.DeletedOn <= DateTimeOffset.MinValue);
            var res = new List<Post>();
            using var iterator = queryable.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var resItems = await iterator.ReadNextAsync(cancellationToken: cancellationToken);
                res.AddRange(resItems.ToList());
            }
            return res;
        }

        public async Task<List<PhotoDTO>> GetPhotosAsync(string containerName, CancellationToken cancellationToken = default)
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

        public async Task<List<PostDTO>> GetPostsAsync(string containerName, CancellationToken cancellationToken = default)
        {
            var con = GetContainer(containerName);
            var res = new List<PostDTO>();
            using var queryable = con.GetItemQueryIterator<BlobMetadata>();
            while (queryable.HasMoreResults)
            {
                var feedResponse = await queryable.ReadNextAsync(cancellationToken: cancellationToken);
                var availableItems = feedResponse.Where(i => i.DeletedOn is null || i.DeletedOn <= DateTimeOffset.MinValue);
                res.AddRange(availableItems.Adapt<List<PostDTO>>());
            }
            return res;
        }

        public async Task<List<PhotoDTO>> GetUserPhotosAsync(string userId, string containerName, CancellationToken cancellationToken = default)
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

        public async Task<bool> DeleteDocumentAsync(T meta, string containerName, CancellationToken cancellationToken = default)
        {
            var container = GetContainer(containerName);
            var result = await container.DeleteItemAsync<BlobMetadata>(meta.Id, new PartitionKey(meta.UserId), cancellationToken: cancellationToken);
            return result == null;
        }

        public async Task<T> SingleAsync(string documentId, string userId, string containerName, CancellationToken cancellationToken = default)
        {
            var container = GetContainer(containerName);
            var blob = await container.ReadItemAsync<T>(documentId, new PartitionKey(userId), cancellationToken: cancellationToken);
            if (blob == null) throw new CosmosDocumentNotFoundException
            {
                ContainerName = containerName,
                DocumentId = documentId,
                UserId = userId
            };

            return blob;
        }

        public Task<T> PatchItemAsync(string id, string partitionKey, string containerName, IReadOnlyList<IPatchOperation<T>> ops, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> PatchItemAsync(string id, string partitionKey, string containerName, IReadOnlyList<Dictionary<string, string?>> ops, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IPostsRepositoryCosmos<T>.ListPostsAsync(string containerName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
