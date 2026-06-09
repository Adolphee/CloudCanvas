using CloudCanvas.Web.Data;

namespace CloudCanvas.Web.Interfaces
{
    public interface IPost : ILikeable, IDisLikeable
    {
        public bool ReportPost(ApplicationUser user, string reason);
        public bool Publish(ApplicationUser user);
        public bool UnPublish(ApplicationUser user);
        public bool Delete(ApplicationUser user, bool softDelete = true);

    }
}
