using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Carts.Queries.CheckCourseInCart;

#region Query

public sealed record CheckCourseInCartQuery(
    Guid? UserId,
    string? SessionId,
    Guid CourseId
) : IQuery<bool>;

#endregion

#region Validator

public sealed class CheckCourseInCartQueryValidator : AbstractValidator<CheckCourseInCartQuery>
{
    public CheckCourseInCartQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
            .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");

        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("شناسه دوره الزامی است");
    }
}

#endregion

#region Handler

internal sealed class CheckCourseInCartQueryHandler(
    ICartRepository cartRepository)
    : IQueryHandler<CheckCourseInCartQuery, bool>
{
    public async Task<Result<bool>> Handle(
        CheckCourseInCartQuery request,
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

        // Return false if cart not found
        if (cart is null)
            return Result.Success(false);

        var courseIdResult = CourseId.CreateFrom(request.CourseId);
        if (courseIdResult.IsFailure)
            return Result.Success(false);

        return Result.Success(cart.ContainsCourse(courseIdResult.Value));
    }
}

#endregion
