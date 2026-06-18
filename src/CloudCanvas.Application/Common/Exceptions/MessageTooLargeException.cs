namespace CloudCanvas.Application.Common.Exceptions
{
    [Serializable]
    public class MessageTooLargeException : Exception
    {
        public int MaxMessageSize { get; set; }
        public int ActualMessageSize { get; set; }
        public string? MessageId { get; internal set; }
        public string? CorrelationId { get; internal set; }

        public MessageTooLargeException(string message) : base(message) { }
        public MessageTooLargeException() { }
        public MessageTooLargeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
