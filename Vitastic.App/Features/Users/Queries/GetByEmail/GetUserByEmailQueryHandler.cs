using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Users.Dtos;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Queries.GetByEmail;

public sealed class GetUserByEmailQueryHandler(
    IUserQueryService userRepository)
    : IQueryHandler<GetUserByEmailQuery, UserDetailDto>
{
    public async Task<Result<UserDetailDto>> Handle(
        GetUserByEmailQuery query,
        CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(query.Email);
        if (emailResult.IsFailure) return emailResult.Error;
        UserDetailDto? user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null)
            return Error.NotFound("GetUserByEmailQueryHandler.UserNotFound", "کاربر یافت نشد.");
        return user;
    }
}
