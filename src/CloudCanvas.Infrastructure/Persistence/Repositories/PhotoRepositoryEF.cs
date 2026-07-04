using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public class PhotoRepositoryEF(CCDBContext ctx) : IPhotoRepositoryEF
    {
        private readonly CCDBContext _contex = ctx;
        public async Task<string?> AddPhotoAsync(Photo photo, CancellationToken cancellation)
        {
            var res = _contex.Add(photo);
            return await _contex.SaveChangesAsync(cancellation) > 0? res.Entity.Id: null;
        }

        public Task<bool> DeletePhotoAsync(string id, CancellationToken cancellation, bool softDelete = true)
        {
            throw new NotImplementedException();
        }

        public Task<Photo?> GetPhotoByIdAsync(string id, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<Photo?> UpdatePhotoAsynce(Photo photo, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
