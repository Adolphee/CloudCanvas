using CloudCanvas.Application.Posts.Photos.Interfaces;

namespace CloudCanvas.Application.Posts.Photos.Commands.UpdatePhoto
{
    public sealed record UpdatePhotoCommandHandler(IPhotoRepository context)
    {
        private readonly IPhotoRepository _context = context;

        public async Task<bool?> Handle(UpdatePhotoCommand command, CancellationToken cancellation = default)
            => await _context.UpdateAsync(command.ToPhoto(), cancellation);
    }
}
