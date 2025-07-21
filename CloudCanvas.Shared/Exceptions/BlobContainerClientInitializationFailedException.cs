namespace CloudCanvas.Shared.Exceptions
{
    [Serializable]
    public class BlobContainerClientInitializationFailedException : Exception
    {
        public BlobContainerClientInitializationFailedException(string message) : base(message) { }
        public BlobContainerClientInitializationFailedException() { }
        public BlobContainerClientInitializationFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
