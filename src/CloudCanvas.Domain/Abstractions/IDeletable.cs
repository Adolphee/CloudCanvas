using CloudCanvas.Domain.User;
namespace CloudCanvas.Domain.Abstractions
{
    public interface IDeletable
    {
        bool Delete(string userId, bool softDelete = true);
    }
}