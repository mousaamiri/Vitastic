using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Users.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Queries.GetAvatarInfo;

public record GetAvatarInfoQuery(Guid UserId):IQuery<UserAvatarInfoDto>;

public sealed class GetAvatarInfoQueryValidation : AbstractValidator<GetAvatarInfoQuery>
{
    public GetAvatarInfoQueryValidation()
        => RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی تواند خالی باشد.");
}
public sealed class GetUserWalletBalanceQueryHandler
(IUserQueryService userQueryService)
    : IQueryHandler<GetAvatarInfoQuery,UserAvatarInfoDto>
{
    public async Task<Result<UserAvatarInfoDto>> Handle(GetAvatarInfoQuery request, CancellationToken cancellationToken)
    {
        Result<UserId>? userIdResult = UserId.CreateFrom(request.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;
        UserAvatarInfoDto? info = await userQueryService.GetUserAvatarInfoAsync(userIdResult.Value, cancellationToken);
        if(info is null )
            return Error.NotFound("GetAvatarInfoQuery.NotFound", "اطلاعات کاربر یافت نشد.");
        return info;
    }
}
