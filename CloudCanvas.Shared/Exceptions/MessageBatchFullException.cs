namespace CloudCanvas.Shared.Exceptions
{
    [Serializable]
    public class MessageBatchFullException : Exception
    {
        public MessageBatchFullException(string message) : base(message) { }
        public MessageBatchFullException() { }
        public MessageBatchFullException(string message, Exception innerException) : base(message, innerException) { }
    }
}
