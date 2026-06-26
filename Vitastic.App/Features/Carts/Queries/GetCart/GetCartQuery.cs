using AutoMapper;
using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Carts.Dtos;
using Vitastic.App.Features.Common.Mapping;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Carts.Queries.GetCart;

#region Query

public sealed record GetCartQuery(
    Guid? UserId,
    string? SessionId
) : IQuery<CartDto>;

#endregion

#region Validator

public sealed class GetCartQueryValidator : AbstractValidator<GetCartQuery>
{
    public GetCartQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
            .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");
    }
}

#endregion

#region Handler

internal sealed class GetCartQueryHandler(
    ICartRepository cartRepository,IMapper mapper)
    : IQueryHandler<GetCartQuery, CartDto>
{
    public async Task<Result<CartDto>> Handle(
        GetCartQuery request,
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

        // Return empty cart if not found
        if (cart is null)
        {
            return Result.Success(new CartDto
            {
                Items = [],
                ItemsCount = 0,
                ItemsTotal = 0
            });
        }

        // Map to DTO
        return mapper.Map<CartDto>(cart);
    }
}

#endregion
