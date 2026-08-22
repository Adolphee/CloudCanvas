using System.Linq.Expressions;

namespace CloudCanvas.Application.Abstractions.Projection
{
    public interface IProjectionStoreBase<T> where T: PostDTO
    {
        Task<T> CreateProjectionAsync(T post, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(T item, bool softDelete = true, CancellationToken cancellationToken = default);
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<T>> GetAllFilteredAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        Task<List<T>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> ReplaceProjectionAsync(T post, CancellationToken cancellation = default);
        Task<T?> SingleAsync(ProjectionKey key, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(ProjectionKey key, CancellationToken cancellationToken = default);
        Task<T> PatchAsync(ProjectionKey key, IDictionary<string, object> ops, CancellationToken cancellationToken = default);
    }
}
