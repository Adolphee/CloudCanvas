using CloudCanvas.Application.Common.Interfaces;
using CloudCanvas.Domain.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CloudCanvas.Infrastructure.Common
{
    public class ImageTool : IImageTool
    {
        public async Task<Stream> ResizeAsync(Stream input, ThumbnailSize size, CancellationToken cancellation = default)
        {
            var output = new MemoryStream();
            var image = await Image.LoadAsync(input, cancellation);
            var pixels = GetPixelSize(size);
            image.Mutate(i => i.Resize(0, pixels)); // zero value makes the resize proportional
            await image.SaveAsJpegAsync(output, cancellation);
            output.Position = 0;
            return output;
        }

        public async Task<Stream> ResizeAsync(Stream input, int height, int width, CancellationToken cancellation = default)
        {
            var output = new MemoryStream();
            var image = await Image.LoadAsync(input, cancellation);
            if(height < image.Height || width < image.Width)
                image.Mutate(i => i.Resize(image.Height > height ? height : 0, image.Width > width ? width : 0));
            await image.SaveAsJpegAsync(output, cancellation);
            output.Position = 0;
            return output;
        }

        public ThumbnailSize GetThumbnailSize(int index)
        {
            ThumbnailSize size = (ThumbnailSize)index;
            return size;
        }
        public int GetPixelSize(ThumbnailSize tnSize)
        {
            return tnSize switch
            {
                ThumbnailSize.xsmall => 25,
                ThumbnailSize.small => 50,
                ThumbnailSize.medium => 75,
                _ => 50,
            };
        }
    }
}
