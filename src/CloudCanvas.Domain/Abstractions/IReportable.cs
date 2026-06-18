using CloudCanvas.Domain.User;

namespace CloudCanvas.Domain.Abstractions
{
    public interface IReportable
    {
        bool ReportPost(IAppUser user, string reason);
    }
}