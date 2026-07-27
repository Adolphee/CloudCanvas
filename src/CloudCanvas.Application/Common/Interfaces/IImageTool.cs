using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Application.Common.Interfaces
{
    public interface IImageTool
    {
        Task<Stream> ResizeAsync(Stream input, ThumbnailSize size, CancellationToken cancellation);
        Task<Stream> ResizeAsync(Stream input, int height, int width, CancellationToken cancellation = default);
        ThumbnailSize GetThumbnailSize(int index);
        int GetPixelSize(ThumbnailSize tnSize);
    }
}
