using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories;

internal class RoleRepository(ApplicationWriteDbContext dbContext) :
    BaseRepository<Role, RoleId>(dbContext), IRoleRepository
{
    public async Task<Role?> FindAsync(RoleId id, CancellationToken cancellation = default)
        => await ExecuteAsync(
            async () =>await Context.Roles
                .Include(r=>r.RolePermissions)
                .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellation),
            cancellation);

    public async Task<bool> ExistsAsync(RoleId roleId, CancellationToken cancellationToken)
        => await ExecuteAsync(() => Context.Roles.AnyAsync(x => x.Id == roleId, cancellationToken), cancellationToken);

    public async Task<Role?> FindByNameAsync(RoleName roleName, CancellationToken cancellationToken)
        => await ExecuteAsync(
            () => Context.Roles.FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken),
            cancellationToken);

    public async Task<bool> NameIsExistAsync(RoleName roleName, CancellationToken cancellationToken)
        => await ExecuteAsync(() => Context.Roles.AnyAsync(x => x.Name == roleName, cancellationToken), cancellationToken);

    public async Task<bool> RoleIsExistedAsync(RoleId roleId, CancellationToken token = default)
        => await ExecuteAsync(() => Context.Roles
            .AnyAsync(x => x.Id == roleId, token), token);

    public async Task<List<Role>> GetUserRolesAsync(UserId userId, CancellationToken token = default)
        => await ExecuteAsync( async () =>
        {
            var userRoles =  await Context.UserRoles
                .Where(ur => ur.UserId.Equals(userId))
                .Select(ur=>ur.RoleId)
                .Distinct()
                .ToListAsync(token);
            var roles =await  Context.Roles
                .Distinct()
                .Where(r => userRoles.Contains(r.Id)).ToListAsync(token);
            return roles;
        }, token);
}
