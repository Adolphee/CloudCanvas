namespace CloudCanvas.Application.Users.Commands.EnsureUserExists
{
    public sealed record EnsureUserExistsCommand(ApplicationUser User): IRequest<EnsureUserExistsResult>
    {
    }
}
