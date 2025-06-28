using CloudCanvas.Shared.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using CloudCanvas.Shared.Constants;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using CloudCanvas.Shared.DTOs;

namespace CloudCanvas.Shared.Services
{
    public class CosmosClientWrapper : ICosmosClientWrapper
    {
        private readonly IConfiguration _config;
        private readonly CosmosClient _client;
        private readonly ILogger<CosmosClientWrapper> _logger;

        ///TODO: Inject the cosmos client pre-configured
        public CosmosClientWrapper(IConfiguration config, ILogger<CosmosClientWrapper> logger)
        {
            _config = config;
            _client = new CosmosClient(_config.GetConnectionString(CloudCosmos.Sql), new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously saves the specified object to the given Cosmos DB container.
        /// </summary>
        /// <remarks>This method performs schema validation on the object using data annotations defined
        /// in the <see cref="MetadataDocumentBase"/> class or its derived types. If validation fails, the errors are
        /// logged, and an <see cref="ArgumentException"/> is thrown. The object is then upserted into the specified
        /// Cosmos DB container.</remarks>
        /// <typeparam name="T">The type of the object to save. Must inherit from <see cref="MetadataDocumentBase"/>.</typeparam>
        /// <param name="obj">The object to be saved. The object must have valid data annotations and non-empty <see
        /// cref="MetadataDocumentBase.Id"/> and <see cref="MetadataDocumentBase.UserId"/> properties.</param>
        /// <param name="containerName">The name of the Cosmos DB container where the object will be saved.</param>
        /// <returns>The saved object as returned by Cosmos DB after the upsert operation.</returns>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="obj"/> is invalid, or if the <see cref="MetadataDocumentBase.Id"/> or <see
        /// cref="MetadataDocumentBase.UserId"/> properties are null, empty, or whitespace.</exception>
        public async Task<T> SaveAsync<T>(T obj, string containerName) where T: MetadataDocumentBase
        {
            /// Custom Schema Validation based on, say, DTO classes before persistence
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();
            // This validates against the dataAnnotations I defined in the abstract parent class MetadataDocumentBase.cs
            bool isValid = Validator.TryValidateObject(obj, context, results, true);
            if(!isValid) results.ForEach(result => _logger.LogError(result.ErrorMessage));
            if (!isValid || string.IsNullOrWhiteSpace(obj.Id) || string.IsNullOrWhiteSpace(obj.UserId))
                throw new ArgumentException("Id and UserId are required fields for CosmosDB persistence.");
            var container = _client.GetContainer(CloudCosmos.Sql, containerName);
            var upsertResult = await container.UpsertItemAsync(obj, new PartitionKey(obj.UserId));
            return upsertResult.Resource;
        }

        public IEnumerable<T> Query<T>(string containerName) where T : MetadataDocumentBase
        {
            var result = _client.GetContainer(CloudCosmos.Sql, containerName).GetItemLinqQueryable<T>();
            return result.AsEnumerable();
        }
    }
}
