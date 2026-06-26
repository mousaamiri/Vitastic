using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories
{
    public interface IUserRoleRepository : IRepository<UserRole, UserRoleId>
    {
        Task<bool> IsUserInRoleAsync(UserId userId, RoleId roleId,CancellationToken token=default);
        Task<UserRole?> FindConnectionAsync(UserId userId, RoleId roleId,CancellationToken token=default);

    }
}
