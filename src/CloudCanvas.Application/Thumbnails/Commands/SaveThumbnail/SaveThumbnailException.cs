namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    [Serializable]
    internal class SaveThumbnailException : Exception
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