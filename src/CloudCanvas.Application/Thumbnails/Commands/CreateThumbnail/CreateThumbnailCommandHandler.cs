using CloudCanvas.Application.Abstractions.Storage;
using CloudCanvas.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
namespace CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail
{
    public sealed class CreateThumbnailCommandHandler(IMediaStorage _filestorage, IImageTool _imageTool, ILogger<CreateThumbnailCommandHandler> logger) : IRequestHandler<CreateThumbnailCommand, CreateThumbnailResult>
    {
        private readonly ILogger<CreateThumbnailCommandHandler> _logger = logger;
        private readonly IMediaStorage _fileService = _filestorage;
        private readonly IImageTool _imageTool = _imageTool;

        public async Task<CreateThumbnailResult> Handle(CreateThumbnailCommand command, CancellationToken cancellationToken)
        {
            using var stream = await _fileService.GetFileStreamFromCommand(command);
            using var thumbnail = await _imageTool.ResizeAsync(stream, command.ThumbnailSize, cancellationToken); // Create thumbnail
            var props = _fileService.SetOriginalMetadata(command.Photo.OriginalFilename, command.Photo.UserId!, cancellationToken);
            FileMetadata thumbnailMeta = await _fileService.UploadAsync(thumbnail, command.Photo.OriginalFilename, props, BStorage.Containers.Thumbnails, $"{command.Photo.Id}_{command.ThumbnailSize.ToString()}", cancellationToken);
            _logger.LogInformation("Created {size} thumbnail for {containerName}/{identifier}", command.ThumbnailSize, command.OriginalContainer, command.Photo.Id);
            command.Photo.Thumbnails.Add(command.ThumbnailSize.ToString(), thumbnailMeta.Location);
            return new CreateThumbnailResult(command.ThumbnailSize, thumbnailMeta.Location, command.Photo);
        }
    }
}
