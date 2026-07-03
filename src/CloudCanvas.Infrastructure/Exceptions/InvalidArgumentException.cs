namespace CloudCanvas.Infrastructure.Exceptions
{
    [Serializable]
    public class InvalidArgumentException : Exception
    {
        public InvalidArgumentException(string message) : base(message) { }
        public InvalidArgumentException() { }
        public InvalidArgumentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
