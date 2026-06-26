using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories;

public interface IPermissionRepository : IRepository<Permission, PermissionId>
{
    Task<bool> PermissionIsExistedAsync(string code,CancellationToken token=default);
    Task<bool> PermissionIsExistedAsync(PermissionId permissionId,CancellationToken token=default);
}
