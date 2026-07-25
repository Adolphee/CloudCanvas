using CloudCanvas.Application.Posts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Abstractions.Projection
{
    public interface IProjectionStoreBase<T> where T: PostDTO
    {
        Task<T> SaveProjectionAsync(T photo, string containerName, bool overWrite = true, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(T item, string containerName, CancellationToken cancellationToken = default);
        Task<List<T>> GetAllAsync(string containerName, CancellationToken cancellationToken = default);
        Task<List<T>> GetByUserIdAsync(string userId, string containerName, CancellationToken cancellationToken = default);
        Task<T> SingleAsync(string documentId, string partitionKey, string containerName, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string containerName, string id, string partitionKey, CancellationToken cancellationToken = default);
        Task<PhotoDTO> PatchAsync(string identifier, string userId, string containerName, IDictionary<string, object> ops, CancellationToken cancellationToken = default);
    }
}
