namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IPublishable
    {
        public bool UnPublish(string userId);
        public bool Publish(string userId);

        protected bool SetPublishedOn();
        protected bool SetUnpublishedOn();
    }
}