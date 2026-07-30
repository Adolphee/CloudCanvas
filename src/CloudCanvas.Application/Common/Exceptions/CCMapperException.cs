namespace CloudCanvas.Application.Common.Exceptions
{
    [Serializable]
    public class CCMapperException: Exception
    {
        public CCMapperException(string message): base(message) { }
        public CCMapperException() { }
        public CCMapperException(string message, Exception innerException): base(message, innerException) { }
    }
}
