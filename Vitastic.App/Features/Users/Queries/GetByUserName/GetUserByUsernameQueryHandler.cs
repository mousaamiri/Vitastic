using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Users.Dtos;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Queries.GetByUserName;

public sealed class GetUserByUsernameQueryHandler(
    IUserQueryService userRepository)
    : IQueryHandler<GetUserByUsernameQuery, UserDetailDto>
{
    public async Task<Result<UserDetailDto>> Handle(
        GetUserByUsernameQuery query,
        CancellationToken cancellationToken)
    {
        var userNameResult = UserName.Create(query.UserName);
        if (userNameResult.IsFailure)
            return userNameResult.Error;
        UserDetailDto? user =  await userRepository.GetByUsernameAsync(userNameResult.Value, cancellationToken);
        if (user is null)
            return Error.NotFound("GetUserByUserNameQuery.UserNotFound", "کاربر یافت نشد.");
        return user;
    }

}
