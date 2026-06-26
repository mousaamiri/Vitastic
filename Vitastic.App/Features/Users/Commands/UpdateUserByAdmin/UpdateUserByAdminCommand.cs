using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.UpdateUserByAdmin;


public sealed record UpdateUserByAdminCommand(
    Guid UserId,
    string UserName,
    string Email,
    string? Password, // optional — if null, password won't change
    string FirstName,
    string LastName,
    string? PhoneNumber,
    IFile? AvatarFile,
    List<Guid> RoleIds,
    bool IsActive
) : ICommand;
