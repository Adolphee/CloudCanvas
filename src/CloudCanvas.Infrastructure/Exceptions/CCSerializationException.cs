namespace CloudCanvas.Infrastructure.Exceptions
{
    [Serializable]
    public class CCSerializationException: Exception
    {
        public CCSerializationException(string message): base(message) { }
        public CCSerializationException() { }
        public CCSerializationException(string message, Exception innerException): base(message, innerException) { }
    }
}
