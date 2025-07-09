using CloudCanvas.Shared.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CloudCanvas.Shared.Utilities
{
    public static class ImageResizer
    {
        public static async Task<Stream> ResizeAsync(Stream input, ImageSize size)
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

        private static int GetPixelSize(ImageSize tnSize)
        {
            switch (tnSize)
            {
                case ImageSize.XS: return 25;
                case ImageSize.S: return 50;
                case ImageSize.M: return 75;
                default: return 50;
            }
        }
    }
}
