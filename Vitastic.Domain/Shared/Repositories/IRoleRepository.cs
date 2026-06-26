using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories
{
    public interface IRoleRepository : IRepository<Role, RoleId>
    {
        Task<bool> ExistsAsync(RoleId roleId, CancellationToken cancellationToken);
        Task<Role?> FindByNameAsync(RoleName roleName, CancellationToken cancellationToken);
        Task<bool> NameIsExistAsync(RoleName roleName, CancellationToken cancellationToken);
        Task<bool> RoleIsExistedAsync(RoleId roleId, CancellationToken token=default);
        Task<List<Role>> GetUserRolesAsync(UserId userId, CancellationToken token=default);
    }
}
