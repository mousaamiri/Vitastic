using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Data;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Carts.Commands.AddCartItem;

#region Command

// UserId is now nullable - if null, use SessionId
public sealed record AddCartItemCommand(
    Guid? UserId,
    string? SessionId,
    Guid CourseId
) : ICommand<Guid>; // Returns CartItemId

#endregion

#region Validator

public sealed class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        // Either UserId or SessionId must be provided
        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
            .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");

        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("شناسه دوره الزامی است");

        RuleFor(x => x.SessionId)
            .MaximumLength(128)
            .When(x => !string.IsNullOrWhiteSpace(x.SessionId))
            .WithMessage("شناسه نشست نامعتبر است");
    }
}

#endregion

#region Handler
internal sealed class AddCartItemCommandHandler(
    ICartRepository cartRepository,
    ICourseRepository courseRepository,
    IUserRepository userRepository,
    IInstructorRepository instructorRepository,
    IUnitOfWork unitOfWork,
    ICartIdentityService cartIdentityService)
    : ICommandHandler<AddCartItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {

        // Determine if user is authenticated or guest
        var identity = cartIdentityService.GetCartIdentity();

        if (!identity.IsAuthenticated && !identity.IsGuest)
            return Error.Failure("Cart.NoIdentity", "شناسه کاربر یا نشست یافت نشد");

        UserId? userId= null;
        if (identity.IsAuthenticated)
        {
            var userIdResult = UserId.CreateFrom(identity.UserId!.Value);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            userId = userIdResult.Value;
        }
        Cart? cart = identity.IsAuthenticated
            ? await cartRepository.GetByUserIdAsync(userId, cancellationToken)
            : await cartRepository.GetBySessionIdAsync(identity.SessionId!, cancellationToken);

        if (cart is null)
        {
            if (identity.IsAuthenticated)
            {
                User? user = await userRepository.FindAsync(userId,cancellationToken);
                if (user is null)
                    return Error.NotFound("AddCartItemCommand.UserNotFound", "کاربر یافت نشد.");
                cart = Cart.Create(user.Id,user.UserFullName,user.Email).Value;
            }
            else
                cart = Cart.CreateForGuest(identity.SessionId!).Value;

            await cartRepository.AddAsync(cart, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var courseIdResult = CourseId.CreateFrom(request.CourseId);
        if (courseIdResult.IsFailure)
            return courseIdResult.Error;

        Course? course = await courseRepository.GetCourseWithSectionAndEpisodes(courseIdResult.Value, cancellationToken);
        if (course is null)
            return CartErrors.InvalidCourse;
        FullName? courseInstructorFullName = await instructorRepository.GetCourseInstructorFullName(course.InstructorId, cancellationToken);
        if (courseInstructorFullName is null)
            return Error.NotFound("AddCartItemCommand.CourseInstructorNotFound", "مدرس دوره یافت نشد.");

        Result<CartItem> itemResult = cart.AddItem(course.Id,
            course.Title,courseInstructorFullName,course.ImageName,course.Price);
        if (itemResult.IsFailure)
            return itemResult.Error;

        await cartRepository.UpdateAsync(cart, cancellationToken);
        return Result.Success(itemResult.Value.Id.Value);
    }
}

#endregion
