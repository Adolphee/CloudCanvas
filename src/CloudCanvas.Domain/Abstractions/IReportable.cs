using CloudCanvas.Domain.User;

namespace CloudCanvas.Domain.Abstractions
{
    public interface IReportable
    {
        bool ReportPost(AppUser user, string reason);
    }
}