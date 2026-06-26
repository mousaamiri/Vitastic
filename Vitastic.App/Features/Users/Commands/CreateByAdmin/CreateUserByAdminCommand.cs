using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.CreateByAdmin;
public sealed record CreateUserByAdminCommand(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    IFile? AvatarFile,
    List<Guid> RoleIds,
    bool IsActive = true
) : ICommand<Guid>;
