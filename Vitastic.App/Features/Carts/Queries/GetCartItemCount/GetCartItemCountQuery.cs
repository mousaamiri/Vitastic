using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Carts.Queries.GetCartItemCount;

#region Query

public sealed record GetCartItemCountQuery(
    Guid? UserId,
    string? SessionId
) : IQuery<int>;

#endregion

#region Validator

public sealed class GetCartItemCountQueryValidator : AbstractValidator<GetCartItemCountQuery>
{
    public GetCartItemCountQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
            .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");
    }
}

#endregion

#region Handler

internal sealed class GetCartItemCountQueryHandler(
    ICartRepository cartRepository)
    : IQueryHandler<GetCartItemCountQuery, int>
{
    public async Task<Result<int>> Handle(
        GetCartItemCountQuery request,
        CancellationToken cancellationToken)
    {
        // Get cart
        Cart? cart = request.UserId.HasValue
            ? await cartRepository.GetByUserIdAsync(
                UserId.CreateFrom(request.UserId.Value).Value,
                cancellationToken)
            : await cartRepository.GetBySessionIdAsync(
                request.SessionId!,
                cancellationToken);

        // Return 0 if cart not found
        return Result.Success(cart?.ItemsCount ?? 0);
    }
}

#endregion
