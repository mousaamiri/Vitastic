using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Data;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Carts.Commands.RemoveCartItem;

#region Command

public sealed record RemoveCartItemCommand(
    Guid? UserId,
    string? SessionId,
    Guid CartItemId
) : ICommand;

#endregion

#region Validator

public sealed class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
            .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");

        RuleFor(x => x.CartItemId)
            .NotEmpty()
            .WithMessage("شناسه آیتم سبد خرید الزامی است");
    }
}

#endregion

#region Handler

internal sealed class RemoveCartItemCommandHandler(
    ICartRepository cartRepository)
    : ICommandHandler<RemoveCartItemCommand>
{
    public async Task<Result> Handle(
        RemoveCartItemCommand request,
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

        if (cart is null)
            return Error.NotFound("Cart.NotFound", "سبد خرید یافت نشد");

        // Remove item
        var itemIdResult = CartItemId.CreateFrom(request.CartItemId);
        if (itemIdResult.IsFailure)
            return itemIdResult.Error;

        Result removeResult = cart.RemoveItem(itemIdResult.Value);
        if (removeResult.IsFailure)
            return removeResult.Error;

        await cartRepository.UpdateAsync(cart, cancellationToken);

        return Result.Success();
    }
}

#endregion
