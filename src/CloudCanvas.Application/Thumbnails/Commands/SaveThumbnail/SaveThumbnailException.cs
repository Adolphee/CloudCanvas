namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    [Serializable]
    public class SaveThumbnailException : Exception
    {
        public SaveThumbnailException()
        {
        }

        public SaveThumbnailException(string? message) : base(message)
        {
        }

        public SaveThumbnailException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}