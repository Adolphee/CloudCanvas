using System.Net;

namespace CloudCanvas.Application.Common.Exceptions
{
    [Serializable]
    public class PostNotFoundException : Exception {
        public HttpStatusCode StatusCode { get; set; }

        public PostNotFoundException()
        {
        }

        public PostNotFoundException(string? message) : base(message)
        {
        }

        public PostNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}