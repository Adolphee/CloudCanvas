using CloudCanvas.Web.Data;

namespace CloudCanvas.Web.Interfaces
{
    public interface IDeletable
    {
        bool Delete(ApplicationUser user, bool softDelete = true);
    }
}