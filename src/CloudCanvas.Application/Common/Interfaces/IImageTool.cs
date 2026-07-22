using CloudCanvas.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

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
