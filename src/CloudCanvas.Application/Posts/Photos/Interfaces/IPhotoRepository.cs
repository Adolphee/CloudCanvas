using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Posts.Photos.Interfaces
{
    public interface IPhotoRepository
    {
        Task<string?> SaveAsync(Photo post, CancellationToken cancellation = default);
        Task<bool> UpdateAsync(Photo post, CancellationToken cancellation = default);
        Task<bool> DeleteAsync(string id, bool softDelete = true, CancellationToken cancellation = default);
        Task<Photo?> GetByIdAsync(string id, CancellationToken cancellation = default);
        Task<bool> ExistsAsync(string id, CancellationToken cancellation = default);
    }
}
