using Vitastic.App.Features.Users.Dtos;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Common.Abstractions.Services.Queries;

public interface IUserQueryService
{
    Task<bool> HasRoleAsync(UserId value, RoleId roleId, CancellationToken cancellationToken);
    Task<UserDetailDto?> GetByEmailAsync(Email queryEmail, CancellationToken cancellationToken);
    Task<UserDetailDto?> GetByIdAsync(UserId value, CancellationToken token);
    Task<UserDetailDto?> GetByUsernameAsync(UserName queryUserName, CancellationToken cancellationToken);
    Task<UserAvatarInfoDto?> GetUserAvatarInfoAsync(UserId id, CancellationToken cancellationToken);
    Task<string?> GetUserAvatarImagePathAsync(UserId id, CancellationToken cancellationToken);
    Task<(IReadOnlyList<UserDto> items, int total)>
        SearchAsync(string searchTerm, int pageNumber, int pageSize, CancellationToken token=default);
}
