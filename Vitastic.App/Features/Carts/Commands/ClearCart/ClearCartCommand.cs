using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Data;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Carts.Commands.ClearCart;

#region Command

public sealed record ClearCartCommand(
    Guid? UserId,
    string? SessionId
) : ICommand;

#endregion

#region Validator

public sealed class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
{
    public ClearCartCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
            .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");
    }
}

#endregion

#region Handler

internal sealed class ClearCartCommandHandler(
    ICartRepository cartRepository)
    : ICommandHandler<ClearCartCommand>
{
    public async Task<Result> Handle(
        ClearCartCommand request,
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

        // Clear cart
        Result clearResult = cart.Clear();
        if (clearResult.IsFailure)
            return clearResult.Error;

        await cartRepository.UpdateAsync(cart, cancellationToken);

        return Result.Success();
    }
}

#endregion
