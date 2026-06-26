using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;
using Microsoft.EntityFrameworkCore;
namespace Vitastic.Infra.Repositories
{
    internal class UserRoleRepository(ApplicationWriteDbContext dbContext)
        : BaseRepository<UserRole, UserRoleId>(dbContext), IUserRoleRepository
    {
        public async Task<bool> IsUserInRoleAsync(UserId userId, RoleId roleId, CancellationToken token = default)
        => await ExecuteAsync(
            async ()=>await Context.UserRoles
                .AnyAsync(x=>x.UserId == userId && x.RoleId == roleId, token), token);

        public async Task<UserRole?> FindConnectionAsync(UserId userId, RoleId roleId, CancellationToken token = default)
            => await ExecuteAsync(async ()=>await Context.UserRoles.FirstOrDefaultAsync(x=>x.UserId == userId && x.RoleId == roleId, token), token);


    }
}
