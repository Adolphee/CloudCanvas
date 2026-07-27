using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Abstractions.Persistence
{
    public interface IPostRepositoryEF<T> where T : Post
    {
        Task<string?> SaveAsync(T post, CancellationToken cancellation = default);
        Task<bool> UpdateAsync(T post, CancellationToken cancellation = default);
        Task<bool> DeleteAsync(string id, bool softDelete = true, CancellationToken cancellation = default);
        Task<T?> GetByIdAsync(string id, CancellationToken cancellation = default);
        Task<bool> ExistsAsync(string id, CancellationToken cancellation = default);
    }
}
