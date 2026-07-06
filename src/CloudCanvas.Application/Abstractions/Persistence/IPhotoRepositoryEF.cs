using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Abstractions.Persistence
{
    public interface IPhotoRepositoryEF
    {
        Task<string?> AddPhotoAsync(Photo photo, CancellationToken cancellation = default);
        Task<Photo?> GetPhotoByIdAsync(string id, CancellationToken cancellation = default);
        Task<bool> DeletePhotoAsync(string id, CancellationToken cancellation = default, bool softDelete = true);
        Task<bool> ExistsAsync(string id, CancellationToken cancellation = default);
        Task<bool> UpdatePhotoAsynce(Photo photo, CancellationToken cancellation = default);
    }
}
