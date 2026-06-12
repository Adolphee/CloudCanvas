using CloudCanvas.Web.Data;

namespace CloudCanvas.Web.Interfaces
{
    public interface IReportable
    {
        bool ReportPost(ApplicationUser user, string reason);
    }
}