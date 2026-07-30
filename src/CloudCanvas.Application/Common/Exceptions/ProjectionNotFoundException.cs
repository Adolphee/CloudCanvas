namespace CloudCanvas.Application.Common.Exceptions
{
    [Serializable]
    public class ProjectionNotFoundException : Exception
    {
        public string? ContainerName { get; init; }
        public string? DocumentId { get; init; }
        public string? UserId { get; init; }

        public ProjectionNotFoundException(string message) : base(message) { }
        public ProjectionNotFoundException() { }
        public ProjectionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
