using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Cosmos;

namespace CloudCanvas.Shared.Services
{
    public class CosmosClientWrapper : ICosmosClientWrapper
    {
        private readonly CosmosClient _client;

        public CosmosClientWrapper(CosmosClient client)
        {
            _client = client;
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
            if (overWrite) result = await container.ReplaceItemAsync(metadata, metadata.Id, new PartitionKey(metadata.UserId));
            else result = await container.UpsertItemAsync(metadata, new PartitionKey(metadata.UserId));
            return result.Resource;
        }

        public Container GetContainer(string containerName)
        {
            Validate.StringValue(nameof(containerName), containerName);
            var result = _client.GetContainer(CloudCosmos.Sql, containerName);
            if (result == null) throw new CosmosContainerNotFoundException($"Container Not Found: '{containerName}'")
            {
                ContainerName = containerName,
                DatabaseName = CloudCosmos.Sql
            };

            return result;
        }



        public async Task<List<T>> ListBlobsAsync<T>(string containerName) where T: CosmosDocumentBase
        {
            var con = GetContainer(containerName);
            var items = con.GetItemQueryIterator<T>(new QueryDefinition("Select * from c"));
            var res = new List<T>();
            while (items.HasMoreResults)
            {
                var resItems = await items.ReadNextAsync();
                res.AddRange(resItems.ToList());
            }
            return res;
        }

        public async Task<bool> DeleteBlobAsync(BlobMetaDTO meta, string containerName)
        {
            Validate.StringValue(nameof(containerName), containerName);
            var con = GetContainer(containerName);
            var result = await con.DeleteItemAsync<BlobMetaDTO>(meta.Id, new PartitionKey(meta.UserId));
            return result == null;
        }

        public async Task<BlobMetaDTO> SingleAsync(string documentId, string userId, string containerName)
        {
            Validate.StringValue(nameof(containerName), containerName);
            Validate.StringValue(nameof(userId), userId);
            Validate.StringValue(nameof(documentId), documentId);
            var con = GetContainer(containerName);
            var blob = await con.ReadItemAsync<BlobMetaDTO>(documentId, new PartitionKey(userId));
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
