using CloudCanvas.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Abstractions.Persistence
{
    public interface IPostRepositoryEF
    {
        Task<Post> SaveAsync(Post post);
        Task<Post> UpdateAsync(Post post);

        Task<bool> DeleteAsync(Post post);
        Task<Post> GetByIdAsync(string id);
    }
}
