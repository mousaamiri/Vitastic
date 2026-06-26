using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Queries.CheckUserRole;

public sealed class CheckUserRoleQueryHandler(
    IUserQueryService userRepository)
    : IQueryHandler<CheckUserRoleQuery, bool>
{
    public async Task<Result<bool>> Handle(
        CheckUserRoleQuery query,
        CancellationToken cancellationToken)
    {
        var userIdResult = UserId.CreateFrom(query.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        var roleIdResult = RoleId.CreateFrom(query.RoleId);
        if (roleIdResult.IsFailure)
            return roleIdResult.Error;
        return await userRepository.HasRoleAsync(userIdResult.Value, roleIdResult.Value, cancellationToken);
    }

}
