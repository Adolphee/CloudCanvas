using CloudCanvas.Application.Users;
using CloudCanvas.Application.Users.Commands.EnsureUserExists;

namespace CloudCanvas.Application.Abstractions.Persistence
{
    public interface IUserRepository
    {
        Task<EnsureUserExistsResult> EnsureUserExistsAsync(ApplicationUser user, CancellationToken cancellation = default);
    }
}
