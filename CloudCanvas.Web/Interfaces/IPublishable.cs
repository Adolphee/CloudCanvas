using CloudCanvas.Web.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Amqp.Framing;

namespace CloudCanvas.Web.Interfaces
{
    public interface IPublishable
    {
        public DateTimeOffset PublishedOn { get; set; }
        public DateTimeOffset UnpublishedOn { get; set; }

        public bool UnPublish(ApplicationUser user);
        public bool Publish(ApplicationUser user);

        public bool SetPublishedOn();
        public bool SetUnpublishedOn();
    }
}