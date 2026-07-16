using CloudCanvas.Application.Common.Interfaces;
using MediatR;
using static CloudCanvas.Application.Common.Constants.BStorage;

namespace CloudCanvas.Application.Posts.Photos.Commands.UploadFile
{
    public class UploadFileCommandHandler(IFileStorageService files) : IRequestHandler<UploadFileCommand, UploadFileResult>
    {
        private readonly IFileStorageService _files = files;
        public async Task<UploadFileResult> Handle(UploadFileCommand command, CancellationToken cancellationToken)
        {
            var file = command.File;
            var props = new Dictionary<string, string>
            {
                { Meta.UploadedBy, command.UserId },
                { Meta.OriginalFilename, file.FileName },
                { Meta.CreatedOn, DateTimeOffset.UtcNow.ToString() },
                { Meta.Container, Containers.Uploads }
            };
            var res = await _files.UploadAsync(command.Stream, Guid.NewGuid().ToString(), props, Containers.Uploads, null, cancellationToken);
            return new UploadFileResult
            {
                FileMetadata = res
            };
        }
    }
}
