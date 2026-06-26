using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.AssignRoleToUser
{
    public sealed record AssignRoleToUserCommand: ICommand
    {

        public Guid UserId{get; init; }
        public Guid RoleId{get; init; }

        public AssignRoleToUserCommand(Guid userId,Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
        public AssignRoleToUserCommand() { }
    }
}
