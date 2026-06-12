using CloudCanvas.Web.Data;

namespace CloudCanvas.Web.Interfaces
{
    public interface IPost : ILikeable, IDisLikeable, IHasTimestamps, IPublishable, IDeletable, IReportable
    {
    }
}
