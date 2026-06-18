using CloudCanvas.Domain.User;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IPublishable
    {
        public DateTimeOffset PublishedOn { get; set; }
        public DateTimeOffset UnpublishedOn { get; set; }

        public bool UnPublish(IAppUser user);
        public bool Publish(IAppUser user);

        public bool SetPublishedOn();
        public bool SetUnpublishedOn();
    }
}