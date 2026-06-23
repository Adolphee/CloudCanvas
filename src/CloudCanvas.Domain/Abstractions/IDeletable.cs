using CloudCanvas.Domain.User;
namespace CloudCanvas.Domain.Abstractions
{
    public interface IDeletable
    {
        bool Delete(AppUser user, bool softDelete = true);
    }
}