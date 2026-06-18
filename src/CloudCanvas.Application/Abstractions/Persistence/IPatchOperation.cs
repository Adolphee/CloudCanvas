using CloudCanvas.Domain.Posts.Contracts;

namespace CloudCanvas.Application.Abstractions.Persistence
{
    public interface IPatchOperation<T> where T : IPost
    {
        string PropertyName { get; set; }
        string PropertyValue { get; set; }
    }
}