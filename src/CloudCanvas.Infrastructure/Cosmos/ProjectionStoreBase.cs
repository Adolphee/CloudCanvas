using System.Linq.Expressions;
using CloudCanvas.Application.Abstractions.Projection;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Infrastructure.Exceptions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Infrastructure.Cosmos
{
    public abstract class ProjectionStoreBase<T>(CosmosClient client, IConfiguration config, ILogger logger) : IProjectionStoreBase<T> where T: PostDTO
    {
        protected readonly CosmosClient _client = client;
        protected readonly IConfiguration _config = config;
        protected readonly ILogger _logger = logger;
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
            string databaseName = _config.GetValue<string>(AppSettings.ProjectionDbName) ?? Projection.Sql; 
            try
            {
                return await EnsureContainerExistsAsync(databaseName, containerId, cancellation);
            }
            catch (Exception e) { 
                throw new CosmosContainerNotFoundException($"Failed to ensure the existence of {containerId} in CosmosDB.", e)
                {
                    ContainerName = containerId,
                    DatabaseName = databaseName
                };
            }
        }
        public abstract Task<T> CreateProjectionAsync(T post, CancellationToken cancellationToken = default);
        public abstract Task<bool> DeleteAsync(T item, bool softDelete = true, CancellationToken cancellationToken = default);
        public abstract Task<bool> ExistsAsync(ProjectionKey key, CancellationToken cancellationToken = default);
        public abstract Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
        public abstract Task<List<T>> GetAllFilteredAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        public abstract Task<List<T>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        public abstract Task<T> PatchAsync(ProjectionKey key, IDictionary<string, object> ops, CancellationToken cancellationToken = default);
        public abstract Task<bool> ReplaceProjectionAsync(T post, CancellationToken cancellation = default);
        public abstract Task<T?> SingleAsync(ProjectionKey key, CancellationToken cancellationToken = default);
    }
}