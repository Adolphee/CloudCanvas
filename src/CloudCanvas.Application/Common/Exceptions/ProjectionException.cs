using System.Net;

namespace CloudCanvas.Application.Common.Exceptions
{
    [Serializable]
    public class ProjectionException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public ProjectionException()
        {
        }

        public ProjectionException(string? message) : base(message)
        {
        }

        public ProjectionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}