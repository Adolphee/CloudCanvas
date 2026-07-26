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

        public async Task<CreateThumbnailResult> Handle(CreateThumbnailCommand command, CancellationToken cancellation = default)
        {
            await using var stream = await _fileService.GetFileStreamFromCommandAsync(command, cancellation);
            await using var thumbnail = await _imageTool.ResizeAsync(stream, command.ThumbnailSize, cancellation); // Create thumbnail
            var props = _fileService.SetOriginalMetadata(command.Photo.OriginalFilename, command.Photo.UserId!);
            FileMetadata thumbnailMeta = await _fileService.UploadAsync(thumbnail, command.Photo.OriginalFilename, props, BStorage.Containers.Thumbnails, $"{command.Photo.Id}_{command.ThumbnailSize.ToString()}", cancellation);
            _logger.LogInformation("Created {size} thumbnail for {containerName}/{identifier}", command.ThumbnailSize, command.OriginalContainer, command.Photo.Id);
            command.Photo.Thumbnails.Add(command.ThumbnailSize.ToString(), thumbnailMeta.Location);
            return new CreateThumbnailResult(command.OriginalContainer,command.ThumbnailSize, thumbnailMeta.Location, command.Photo);
        }
    }
}
