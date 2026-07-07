using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Posts;
using Mapster;

namespace CloudCanvas.Application.Posts.Photos.Commands.UpdatePhoto
{
    public sealed record UpdatePhotoCommandHandler(IPhotoRepository context)
    {
        private readonly IPhotoRepository _context = context;

        public async Task<bool?> Handle(UpdatePhotoCommand command, CancellationToken cancellation)
            => await _context.UpdateAsync(command.Adapt<Photo>(), cancellation);
    }
}
