
using Microsoft.Extensions.Logging;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Vitastic.Infra.Repositories;

internal class PermissionRepository(
    ApplicationWriteDbContext dbContext,
    ILogger<PermissionRepository> logger)
    : BaseRepository<Permission,PermissionId>(dbContext),
        IPermissionRepository
{
    public async Task<bool> PermissionIsExistedAsync(string code, CancellationToken token = default)
        => await ExecuteAsync(async () => await Context.Permissions.AnyAsync(p => p.Code.ToUpper() == code.ToUpper(), token), token);

    public async Task<bool> PermissionIsExistedAsync(PermissionId permissionId, CancellationToken token = default)
        => await ExecuteAsync(async () => await Context.Permissions.AnyAsync(p => p.Id == permissionId, token), token);
}
