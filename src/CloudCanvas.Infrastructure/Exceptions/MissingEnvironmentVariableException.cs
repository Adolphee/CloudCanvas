namespace CloudCanvas.Infrastructure.Exceptions
{
    [Serializable]
    public class MissingEnvironmentVariableException : Exception
    {
        public MissingEnvironmentVariableException(string message) : base(message) { }
        public MissingEnvironmentVariableException() { }
        public MissingEnvironmentVariableException(string message, Exception innerException) : base(message, innerException) { }
    }
}
