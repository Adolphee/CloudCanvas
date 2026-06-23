using CloudCanvas.Domain.User;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IPublishable
    {
        public DateTimeOffset PublishedOn { get; set; }
        public DateTimeOffset UnpublishedOn { get; set; }

        public bool UnPublish(AppUser user);
        public bool Publish(AppUser user);

        public bool SetPublishedOn();
        public bool SetUnpublishedOn();
    }
}