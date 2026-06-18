using CloudCanvas.Domain.Posts;
using Microsoft.Azure.Cosmos;

namespace CloudCanvas.Infrastructure.Cosmos
{
    public interface IICosmosClientWrapper
    {
        Task<T> SaveMetadataAsync<T>(T obj, string containerName, bool overWrite = false) where T : MetadataDocumentBase;

        Task<bool> DeleteDocumentAsync<T>(T item, string containerName) where T : MetadataDocumentBase;

        Task<List<T>> ListBlobsAsync<T>(string containerName) where T : CosmosDocumentBase;
        Task<List<T>> ListPostsAsync<T>(string containerName) where T : Post;

        Task<T> SingleAsync<T>(string documentId, string partitionKey, string containerName) where T : MetadataDocumentBase;
        Task<bool> ExistsAsync(string containerName, string id, string partitionKey);
        Task<T> PatchItemAsync<T>(string id, string partitionKey, string containerName, IReadOnlyList<PatchOperation> ops) where T : MetadataDocumentBase;
    }
}
