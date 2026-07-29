namespace CloudCanvas.Application.Abstractions.Projection
{
    public interface IProjectionStoreBase<T> where T: PostDTO
    {
        Task<T> CreateProjectionAsync(T photo, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(T item, CancellationToken cancellationToken = default);
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<T>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> ReplaceProjectionAsync(PhotoDTO photo, CancellationToken cancellation = default);
        Task<T> SingleAsync(string documentId, string partitionKey, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string id, string partitionKey, CancellationToken cancellationToken = default);
        Task<PhotoDTO> PatchAsync(string identifier, string userId, IDictionary<string, object> ops, CancellationToken cancellationToken = default);
    }
}
