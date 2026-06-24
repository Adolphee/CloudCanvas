namespace CloudCanvas.Domain.Abstractions
{
    public interface IReportable
    {
        bool ReportPost(string user, string reason = "No reason.");
    }
}