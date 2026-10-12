/// <summary>
/// Simple class to safely hold constant string values.
/// </summary>
namespace CloudCanvas.Application.Common.Constants
{
    public abstract class Unknown
    {
        public const string Username = "unknown_user";
        public const string DisplayName = "Unknown User";
    }

    public abstract class SQLServer
    {
        public const string ConnectionString = "sqlserver";
    }

    public abstract class CCClaimTypes
    {
        public const string ObjectIdentfier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string Name = "name";
    }
}