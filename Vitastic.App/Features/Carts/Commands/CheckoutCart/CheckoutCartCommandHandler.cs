using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Data;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Carts.Commands.CheckoutCart;

#region Handler

internal sealed class CheckoutCartCommandHandler(
    ICartRepository cartRepository,
    ICourseRepository courseRepository,
    IInstructorRepository instructorRepository,
    IUserRepository userRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CheckoutCartCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CheckoutCartCommand request,
        CancellationToken cancellationToken)
    {
        #region 1. Parse & Fetch User

        // Parse the raw Guid into a strongly-typed UserId value object
        var userIdResult = UserId.CreateFrom(request.UserId);

        if (userIdResult.IsFailure)
            return userIdResult.Error;

        var userId = userIdResult.Value;

        // Fetch the User aggregate — needed to extract Value Objects
        User? user = await userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
            return CheckoutCartErrors.UserNotFound;

        #endregion

        #region 2. Fetch Cart & Domain Validation

        // Fetch the cart belonging to this user
        Cart? cart = await cartRepository.GetByUserIdAsync(userId, cancellationToken);

        if (cart is null)
            return CheckoutCartErrors.CartNotFound;

        // Delegate checkout eligibility to the Cart aggregate itself
        Result validationResult = cart.ValidateForCheckout(userId);

        if (validationResult.IsFailure)
            return validationResult.Error;

        #endregion

        #region 3. Get or Create Order

        // Check if user already has a pending order to avoid duplicates
        Order? existingOrder = await orderRepository
            .GetPendingOrderByUserIdAsync(userId, cancellationToken);

        Order order;

        if (existingOrder is not null)
        {
            order = existingOrder;
        }
        else
        {
            // Extract Value Objects from User — Cart never touches User entity
            Result<Order> orderResult = Order.Create(
                user.Id,
                user.UserFullName,
                user.Email,
                user.PhoneNumber);

            if (orderResult.IsFailure)
                return orderResult.Error;

            order = orderResult.Value;
        }

        #endregion

        #region 4. Add Cart Items to Order

        foreach (CartItem cartItem in cart.Items)
        {
            // Skip items already present in the order
            if (order.ContainsCourse(cartItem.CourseId))
                continue;

            // Fetch course aggregate — needed for fresh snapshot data
            Course? course = await courseRepository.GetCourseWithSectionAndEpisodes(
                cartItem.CourseId, cancellationToken);

            if (course is null)
                return CheckoutCartErrors.CourseNotFound(cartItem.CourseId.Value);

            // Fetch instructor name as a Value Object snapshot
            FullName? instructorName = await instructorRepository
                .GetCourseInstructorFullName(course.InstructorId, cancellationToken);

            if (instructorName is null)
                return CheckoutCartErrors.InstructorNotFound(course.Title.Value);

            // Pass only Value Objects to Order — never the Course entity
            Result<OrderItem> addResult = order.AddItem(
                course.Id,
                course.Title,
                course.ImageName,
                instructorName,
                course.Price);

            if (addResult.IsFailure)
                return addResult.Error;
        }

        #endregion

        #region 5. Persist Changes (Unit of Work)

        // Persist order (add or update based on existence)
        if (existingOrder is not null)
            await orderRepository.UpdateAsync(order,cancellationToken);
        else
            await orderRepository.AddAsync(order, cancellationToken);

        // Clear cart items and mark cart as completed via domain method
        cart.Clear();
        await cartRepository.UpdateAsync(cart,cancellationToken);

        // Commit everything in a single transaction
        await unitOfWork.SaveChangesAsync(cancellationToken);

        #endregion

        return Result.Success(order.Id.Value);
    }
}

#endregion

#region Errors

internal static class CheckoutCartErrors
{
    public static readonly Error UserNotFound =
        Error.NotFound(
            "CheckoutCart.UserNotFound",
            "کاربر یافت نشد.");

    public static readonly Error CartNotFound =
        Error.NotFound(
            "CheckoutCart.CartNotFound",
            "سبد خرید یافت نشد.");

    public static Error CourseNotFound(Guid courseId) =>
        Error.NotFound(
            "CheckoutCart.CourseNotFound",
            $"دوره با شناسه {courseId} یافت نشد.");

    public static Error InstructorNotFound(string courseTitle) =>
        Error.NotFound(
            "CheckoutCart.InstructorNotFound",
            $"مدرس دوره «{courseTitle}» یافت نشد.");
}

#endregion
