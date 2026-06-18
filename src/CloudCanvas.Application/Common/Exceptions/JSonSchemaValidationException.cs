using Newtonsoft.Json;

namespace CloudCanvas.Application.Common.Exceptions
{
    [Serializable]
    public class JSonSchemaValidationException : JsonReaderException
    {
        public JSonSchemaValidationException(string message) : base(message) { }
        public JSonSchemaValidationException() { }
        public JSonSchemaValidationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
