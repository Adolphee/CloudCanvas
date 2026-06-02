using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;

namespace CloudCanvas.Shared.Services
{
    public class CosmosClientWrapper : ICosmosClientWrapper
    {
        private readonly CosmosClient _client;
        private Container? _container;

        public CosmosClientWrapper(CosmosClient client)
        {
            _client = client;
        }

        public async Task<T> PatchItemAsync<T>(string identifier, string userId, string containerName, IReadOnlyList<PatchOperation> ops) where T : MetadataDocumentBase
        {
            Validate.StringValue(nameof(identifier), identifier);
            Validate.StringValue(nameof(userId), userId);
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
        public async Task<T> SaveMetadataAsync<T>(T metadata, string containerName, bool overWrite = false) where T: MetadataDocumentBase
        {
            Validate.StringValue(nameof(containerName), containerName);
            Validate.Object(metadata);
            var container = _client.GetContainer(CloudCosmos.Sql, containerName);
            ItemResponse<T> result;
            var partitionKey = new PartitionKey(metadata.UserId);
            if (overWrite) result = await container.ReplaceItemAsync(metadata, metadata.Id, partitionKey);
            else result = await container.CreateItemAsync(metadata, partitionKey);
            return result.Resource;
        }

        public async Task<bool> MetaExistsAsync(string containerName, string id, string partitionKey)
        {
            var container = GetContainer(containerName);
            try
            {
                await container.ReadItemAsync<BlobMetaDTO>(id, new PartitionKey(partitionKey));
                return true;
            }
            catch (CosmosException) //when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public Container GetContainer(string containerId)
        {
            Validate.StringValue(nameof(containerId), containerId);
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

        public async Task<List<T>> ListBlobsAsync<T>(string containerName) where T: CosmosDocumentBase
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

        public async Task<bool> DeleteDocumentAsync<T>(T meta, string containerName = CloudCosmos.Containers.BlobMeta) 
            where T: MetadataDocumentBase
        {
            var container = GetContainer(containerName);
            var result = await container.DeleteItemAsync<BlobMetaDTO>(meta.Id, new PartitionKey(meta.UserId));
            return result == null;
        }

        public async Task<T> SingleAsync<T>(string documentId, string userId, string containerName = CloudCosmos.Containers.BlobMeta)
            where T: MetadataDocumentBase
        {
            Validate.StringValue(nameof(containerName), containerName);
            Validate.StringValue(nameof(userId), userId);
            Validate.StringValue(nameof(documentId), documentId);
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
    }
}
