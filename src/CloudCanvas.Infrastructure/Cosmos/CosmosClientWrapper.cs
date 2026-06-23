using CloudCanvas.Domain.Posts;
using CloudCanvas.Infrastructure.DTOs;
using CloudCanvas.Infrastructure.Exceptions;
using CloudCanvas.Application.Abstractions.Persistence;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CloudCanvas.Infrastructure.Cosmos
{
    public class CosmosClientWrapper<T> : IPostsRepository<T> where T : Post
    {
        private readonly CosmosClient _client;
        private Container? _container;
        public CosmosClientWrapper(CosmosClient client)
        {
            _client = client;
        }

        public async Task<T> PatchItemAsync(string identifier, string userId, string containerName, IReadOnlyList<PatchOperation> ops)
        {
            _container = GetContainer(containerName);
            var metadata = await _container.PatchItemAsync<T>(id: identifier, partitionKey: new PartitionKey(userId), patchOperations: ops);
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
        public async Task<T> SaveMetadataAsync(T metadata, string containerName, bool overWrite = true)
        {
            var container = _client.GetContainer(CloudCosmos.Sql, containerName);
            ItemResponse<T> result;
            var partitionKey = new PartitionKey(metadata.UserId);
            if (overWrite) result = await container.ReplaceItemAsync(metadata, metadata.Id, partitionKey);
            else result = await container.CreateItemAsync(metadata, partitionKey);
            return result.Resource;
        }

        public async Task<bool> ExistsAsync(string containerName, string id, string partitionKey)
        {
            var container = GetContainer(containerName);
            try
            {
                await container.ReadItemAsync<BlobMetaDTO>(id, new PartitionKey(partitionKey));
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

        public async Task<List<T>> ListBlobsAsync(string containerName)
        {
            var con = GetContainer(containerName);
            var queryable = con.GetItemLinqQueryable<T>().Where(x => x.DeletedOn == null);
            var res = new List<T>();
            using var iterator = queryable.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var resItems = await iterator.ReadNextAsync();
                res.AddRange(resItems.ToList());
            }
            return res;
        }

        public async Task<List<Post>> GetPostsAsync(string containerName)
        {
            var con = GetContainer(containerName);
            var res = new List<Post>();
            using var queryable = con.GetItemQueryIterator<BlobMetaDTO>();
            while (queryable.HasMoreResults)
            {
                var resItems = await queryable.ReadNextAsync();
                res.AddRange(resItems.ToList().Select(item => item.ToPost()));
            }
            return res;
        }

        public async Task<bool> DeleteDocumentAsync(T meta, string containerName = CloudCosmos.Containers.BlobMeta)
        {
            var container = GetContainer(containerName);
            var result = await container.DeleteItemAsync<BlobMetaDTO>(meta.Id, new PartitionKey(meta.UserId));
            return result == null;
        }

        public async Task<T> SingleAsync(string documentId, string userId, string containerName = CloudCosmos.Containers.BlobMeta)
        {
            var container = GetContainer(containerName);
            var blob = await container.ReadItemAsync<T>(documentId, new PartitionKey(userId));
            if (blob == null) throw new CosmosDocumentNotFoundException
            {
                ContainerName = containerName,
                DocumentId = documentId,
                UserId = userId
            };

            return blob;
        }

        public Task<T> PatchItemAsync(string id, string partitionKey, string containerName, IReadOnlyList<IPatchOperation<T>> ops)
        {
            throw new NotImplementedException();
        }

        public Task<T> PatchItemAsync(string id, string partitionKey, string containerName, IReadOnlyList<Dictionary<string, string?>> ops)
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IPostsRepository<T>.ListPostsAsync(string containerName)
        {
            throw new NotImplementedException();
        }
    }
}
