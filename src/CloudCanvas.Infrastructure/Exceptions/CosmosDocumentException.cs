namespace CloudCanvas.Infrastructure.Exceptions
{
    [Serializable]
    public class CosmosDocumentException : Exception
    {
        public CosmosDocumentException(string message) : base(message) { }
        public CosmosDocumentException() { }
        public CosmosDocumentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
