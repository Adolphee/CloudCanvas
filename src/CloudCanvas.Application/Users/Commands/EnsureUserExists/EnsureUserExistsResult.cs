using CloudCanvas.Domain.Abstractions.Identity;

namespace CloudCanvas.Application.Users.Commands.EnsureUserExists
{
    public sealed record EnsureUserExistsResult(ApplicationUser User, bool UserExists);
}