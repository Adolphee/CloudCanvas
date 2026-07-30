using CloudCanvas.Application.Abstractions.Persistence;

namespace CloudCanvas.Application.Users.Commands.EnsureUserExists
{
    public class EnsureUserExistsCommandHandler(IUserRepository userRepo) : IRequestHandler<EnsureUserExistsCommand, EnsureUserExistsResult>
    {
        private readonly IUserRepository _repo = userRepo;
        public async Task<EnsureUserExistsResult> Handle(EnsureUserExistsCommand request, CancellationToken cancellationToken = default)
        {
            return await _repo.EnsureUserExistsAsync(request.User, cancellationToken);
        }
    }
}
