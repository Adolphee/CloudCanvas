using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        /// <returns>The saved object as returned by Cosmos DB after the upsert operation.</returns>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="metadata"/> is invalid, or if the <see cref="MetadataDocumentBase.Id"/> or <see
        /// cref="MetadataDocumentBase.UserId"/> properties are null, empty, or whitespace.</exception>
        public async Task<T> SaveMetadataAsync<T>(T metadata, string containerName) where T: MetadataDocumentBase
        {
            Validate.StringValue(nameof(containerName), containerName);
            Validate.MetadataDocumentBase(metadata);
            var container = _client.GetContainer(CloudCosmos.Sql, containerName);
            var upsertResult = await container.UpsertItemAsync(metadata, new PartitionKey(metadata.UserId));
            return upsertResult.Resource;
        }

        public IEnumerable<T> QueryContainer<T>(string containerName) where T : MetadataDocumentBase
        {
            Validate.StringValue(nameof(containerName), containerName);
            var result = _client.GetContainer(CloudCosmos.Sql, containerName).GetItemLinqQueryable<T>();
            return result.AsEnumerable();
        }
    }
}
