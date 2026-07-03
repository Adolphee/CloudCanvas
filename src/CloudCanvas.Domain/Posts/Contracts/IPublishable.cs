namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IPublishable
    {
        public DateTimeOffset PublishedOn { get; set; }
        public DateTimeOffset UnpublishedOn { get; set; }

        public bool UnPublish(string userId);
        public bool Publish(string userId);

        public bool SetPublishedOn();
        public bool SetUnpublishedOn();
    }
}