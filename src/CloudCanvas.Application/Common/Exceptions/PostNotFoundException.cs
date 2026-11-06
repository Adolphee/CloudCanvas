using System.Net;

namespace CloudCanvas.Application.Common.Exceptions
{
    public class NotFoundException : PostNotFoundException
    {
        public NotFoundException(string? message) : base(message)
        {
            StatusCode = HttpStatusCode.NotFound;
        }
    }

    [Serializable]
    public class PostNotFoundException : Exception {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.NotFound;

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