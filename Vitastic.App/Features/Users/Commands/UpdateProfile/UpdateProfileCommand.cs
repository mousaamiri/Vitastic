using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Users.Commands.UpdateProfile
{
    public sealed record UpdateProfileCommand(
        Guid UserId,
        string? FirstName = null,
        string? LastName = null,
        string? PhoneNumber=null) : ICommand;
}
