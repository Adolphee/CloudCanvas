using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Queries.GetAllPosts;
using CloudCanvas.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public class PostRepositoryEF<T>: IPostRepositoryEF where T : Post
    {
        private CCDBContext _context;
        public PostRepositoryEF(CCDBContext ctx)
        {
            _context = ctx;
        }

        public Task<bool> DeleteAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Post> SaveAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<Post> UpdateAsync(Post post)
        {
            throw new NotImplementedException();
        }

        /*public async Task<PhotoDTO> GetSignleByIdAsync(string id)
        {
        }*/
    }
}
