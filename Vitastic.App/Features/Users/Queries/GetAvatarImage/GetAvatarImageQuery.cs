using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Users.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Queries.GetAvatarImage;

public record GetAvatarImageQuery(Guid UserId):IQuery<string>;

public sealed class GetAvatarImageQueryValidation : AbstractValidator<GetAvatarImageQuery>
{
    public GetAvatarImageQueryValidation()
        => RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی تواند خالی باشد.");
}
public sealed class GetUserWalletBalanceQueryHandler
(IUserQueryService userQueryService)
    : IQueryHandler<GetAvatarImageQuery,string>
{
    public async Task<Result<string>> Handle(GetAvatarImageQuery request, CancellationToken cancellationToken)
    {
        Result<UserId>? userIdResult = UserId.CreateFrom(request.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;
        string? path = await userQueryService.GetUserAvatarImagePathAsync(userIdResult.Value, cancellationToken);
        if(path is null )
            return Error.NotFound("GetAvatarImageQuery.NotFound", "اطلاعات کاربر یافت نشد.");
        return path;
    }
}
