using CloudCanvas.Application.Abstractions.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Cosmos.IPostsRepositoryCosmos<CloudCanvas.Domain.Posts.Contracts.IPost>;

namespace CloudCanvas.Application.Posts.Photos.Commands.UpdatePhoto
{
    public sealed record UpdatePhotoCommandHandler(IPhotoRepositoryEF context, ICosmosRepo cosmos)
    {
        private readonly IPhotoRepositoryEF _context = context;
        private readonly ICosmosRepo _cosmos = cosmos;

        public async Task<bool?> Handle(UpdatePhotoCommand command, CancellationToken cancellation)
        {

            return false;
        }
    }
}
