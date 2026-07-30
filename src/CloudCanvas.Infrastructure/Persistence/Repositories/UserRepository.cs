using CloudCanvas.Application.Abstractions.Identity;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Users.Commands.EnsureUserExists;
using CloudCanvas.Infrastructure.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public class UserRepository(CCDBContext context): IUserRepository
    {
        private readonly CCDBContext _context = context;

        public async Task<EnsureUserExistsResult> EnsureUserExistsAsync(ApplicationUser user, CancellationToken cancellation = default)
        {

            if (!await ExistsAsync(user.Id, cancellation))
            {
                var dbUser = await _context.AddAsync(user.ToIdentityUser(), cancellation);
                try
                {
                    var res = await _context.SaveChangesAsync(default, cancellation);
                    return new EnsureUserExistsResult(dbUser.Entity.ToAppUser(), false);
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                { // FK Constraint violation, race condition: since calling ExistsAsync(),
                  // this user has already been inserted some other way
                  // We have ensured the user exists; moving on...
                }
            }

            var identityUser = await _context.Users.FindAsync(user.Id, cancellation);
            return new EnsureUserExistsResult(identityUser!.ToAppUser(), true);
        }

        public async Task<bool> ExistsAsync(string userId, CancellationToken cancellation = default)
        => await _context.Users.AnyAsync(u => u.Id == userId, cancellation);
    }
}
