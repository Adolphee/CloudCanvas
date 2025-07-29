using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using Microsoft.Azure.Cosmos;

namespace CloudCanvas.Shared.Interfaces
{
    /// <summary>
    /// Defines an abstraction for interacting with a Cosmos DB client, providing methods to save and query objects in
    /// specified containers.
    /// </summary>
    /// <remarks>This interface is designed to simplify operations with Cosmos DB by abstracting common tasks
    /// such as saving objects and querying containers. Implementations of this interface should handle the underlying
    /// communication with Cosmos DB and ensure proper error handling.</remarks>
    public interface ICosmosClientWrapper
    {
        /// <summary>
        /// Saves the specified object to the given container asynchronously.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to persist the object in the specified
        /// container.  Ensure that the container name is valid and accessible before calling this method.</remarks>
        /// <typeparam name="T">The type of the object to save.</typeparam>
        /// <param name="obj">The object to be saved. Cannot be <see langword="null"/>.</param>
        /// <param name="containerName">The name of the container where the object will be saved. Cannot be <see langword="null"/> or empty.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the saved object.</returns>
        Task<T> SaveMetadataAsync<T>(T obj, string containerName, bool overWrite = false) where T : MetadataDocumentBase;

        /// <summary>
        /// Retrieves a collection of items of the specified type from the given container.
        /// </summary>
        /// <remarks>This method queries the specified container and returns all items of the specified
        /// type. Ensure that the container exists and contains items of the expected type to avoid runtime
        /// issues.</remarks>
        /// <typeparam name="T">The type of items to retrieve from the container.</typeparam>
        /// <param name="containerName">The name of the container to query. Must not be null or empty.</param>
        /// <returns>An <see cref="Container"/> The specified container. Returns an empty collection if the container is empty or does not exist.</returns>
        Task<bool> DeleteDocumentAsync<T>(T metaDTO, string containerName) where T: MetadataDocumentBase;

        //Task<bool> SoftDeleteItemAsync<T>(T meta, string containerName) where T : MetadataDocumentBase;

        Task<List<T>> ListBlobsAsync<T>(string containerName) where T: CosmosDocumentBase;

        Task<T> SingleAsync<T>(string documentId, string userId, string containerName) where T: MetadataDocumentBase;
    }
}
