namespace CloudCanvas.Infrastructure.Exceptions
{
    [Serializable]
    public class CosmosDocumentNotFoundException : Exception
    {
        public string? ContainerName { get; internal set; }
        public string? DocumentId { get; internal set; }
        public string? UserId { get; internal set; }

        public CosmosDocumentNotFoundException(string message) : base(message) { }
        public CosmosDocumentNotFoundException() { }
        public CosmosDocumentNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
