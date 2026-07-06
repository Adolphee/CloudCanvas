using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Domain.Posts;
using Microsoft.EntityFrameworkCore;
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
            var res = _contex.Photos.Add(photo);
            return await _contex.SaveChangesAsync(cancellation) > 0? res.Entity.Id: null;
        }

        public Task<bool> DeletePhotoAsync(string id, CancellationToken cancellation, bool softDelete = true)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellation = default) 
            => await _contex.Photos.AnyAsync(x => x.Id == id, cancellation);

        public async Task<Photo?> GetPhotoByIdAsync(string id, CancellationToken cancellation = default)
            => await _contex.Photos.FindAsync(id, cancellation);

        public async Task<bool> UpdatePhotoAsynce(Photo photo, CancellationToken cancellation = default){
            _contex.Update(photo);
            return await _contex.SaveChangesAsync(cancellation) == 1;
        }
    }
}
