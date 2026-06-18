using CloudCanvas.Domain.User;
namespace CloudCanvas.Domain.Abstractions
{
    public interface IDeletable
    {
        bool Delete(IAppUser user, bool softDelete = true);
    }
}