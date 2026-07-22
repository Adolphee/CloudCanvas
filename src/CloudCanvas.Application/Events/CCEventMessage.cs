namespace CloudCanvas.Application.Events
{
    public record CCEventMessage
    {
        public string Subject { get; set; } = default!;
        public string CorrelationId { get; set; } = default!;
        public string EventType { get; set; } = default!;
        public Dictionary<string, object> Properties { get; set; } = new();
        public object Payload { get; set; } = default!;
    }
}
