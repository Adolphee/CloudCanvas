using CloudCanvas.Shared.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CloudCanvas.Shared.Utilities
{
    public static class ImageTool
    {
        public static async Task<Stream> ResizeAsync(Stream input, ThumbnailSize size)
        {
            var output = new MemoryStream();
            var image = await Image.LoadAsync(input);
            var pixels = GetPixelSize(size);
            image.Mutate(i => i.Resize(0, pixels)); // zero value makes the resize proportional
            await image.SaveAsJpegAsync(output);
            output.Position = 0;
            return output;
        }

        public static async Task<Stream> ResizeAsync(Stream input, int height, int width)
        {
            var output = new MemoryStream();
            var image = await Image.LoadAsync(input);
            if(height < image.Height || width < image.Width)
                image.Mutate(i => i.Resize(image.Height > height ? height : 0, image.Width > width ? width : 0));
            await image.SaveAsJpegAsync(output);
            output.Position = 0;
            return output;
        }

        public static ThumbnailSize GetThumbnailSize(int index)
        {
            Validate.Number(nameof(ThumbnailSize), index, 2);
            ThumbnailSize size = (ThumbnailSize)index;
            return size;
        }
        private static int GetPixelSize(ThumbnailSize tnSize)
        {
            switch (tnSize)
            {
                case ThumbnailSize.XSmall: return 25;
                case ThumbnailSize.Small: return 50;
                case ThumbnailSize.Medium: return 75;
                default: return 50;
            }
        }
    }
}
