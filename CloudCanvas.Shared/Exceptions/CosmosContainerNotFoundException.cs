namespace CloudCanvas.Shared.Exceptions
{
    [Serializable]
    public class CosmosContainerNotFoundException : Exception
    {
        public string? ContainerName { get; internal set; }
        public string? DatabaseName { get; internal set; }
        public CosmosContainerNotFoundException(string message) : base(message) { }
        public CosmosContainerNotFoundException() { }
        public CosmosContainerNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
