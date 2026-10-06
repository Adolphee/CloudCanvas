namespace CloudCanvas.Application.Events
{
    public record CCEventMessage
    {
        public string Id { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public string CorrelationId { get; set; } = default!;
        public string SessionId { get; set; } = default!;
        public string EventType { get; set; } = default!;
        public Dictionary<string, object> Properties { get; set; } = new();
        public required BinaryData Payload { get; set; } = default!;
    }
}
