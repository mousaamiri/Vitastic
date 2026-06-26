using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Carts.Requests;
using Vitastic.Api.Features.Carts.Responses;
using Vitastic.App.Features.Carts.Commands.AddCartItem;
using Vitastic.App.Features.Carts.Commands.CheckoutCart;
using Vitastic.App.Features.Carts.Commands.ClearCart;
using Vitastic.App.Features.Carts.Commands.RemoveCartItem;
using Vitastic.App.Features.Carts.Dtos;
using Vitastic.App.Features.Carts.Queries.CheckCourseInCart;
using Vitastic.App.Features.Carts.Queries.GetCart;
using Vitastic.App.Features.Carts.Queries.GetCartItemCount;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Features.Carts;

#region Controller

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CartsController(
    IMediator mediator,
    IMapper mapper,
    ILogger<CartsController> logger) : ControllerBase
{
    // ======================== COMMANDS ========================

    #region Add Cart Item

    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse<Guid>> AddCartItem(
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        var sessionId = HttpContext.GetSessionId();

        logger.LogInformation(
            "Adding item to cart - UserId: {UserId}, SessionId: {SessionId}, CourseId: {CourseId}",
            userId, sessionId, request.CourseId);

        AddCartItemCommand command = new(userId, sessionId, request.CourseId);
        Result<Guid> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Add cart item failed - UserId: {UserId}, CourseId: {CourseId}, Error: {Error}",
                userId, request.CourseId, result.Error);

            return result.ToApiResponse<Guid>("خطا در افزودن دوره به سبد خرید");
        }

        logger.LogInformation(
            "Cart item added - UserId: {UserId}, CartItemId: {CartItemId}",
            userId, result.Value);

        return result.ToApiResponse(
            id => id,
            "دوره با موفقیت به سبد خرید اضافه شد");
    }

    #endregion

    #region Remove Cart Item

    [HttpDelete("items/{itemId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveCartItem(
        Guid itemId,
        CancellationToken cancellationToken)
    {
         var userId = User.GetCurrentUserId();
       var sessionId = HttpContext.GetSessionId();

        logger.LogInformation(
            "Removing cart item - UserId: {UserId}, SessionId: {SessionId}, ItemId: {ItemId}",
            userId, sessionId, itemId);

        RemoveCartItemCommand command = new(userId, sessionId, itemId);
        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Remove cart item failed - UserId: {UserId}, ItemId: {ItemId}, Error: {Error}",
                userId, itemId, result.Error);

            return result.ToApiResponse("خطا در حذف آیتم از سبد خرید");
        }

        logger.LogInformation(
            "Cart item removed - UserId: {UserId}, ItemId: {ItemId}",
            userId, itemId);

        return result.ToApiResponse("آیتم با موفقیت از سبد خرید حذف شد");
    }

    #endregion

    #region Clear Cart

    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> ClearCart(CancellationToken cancellationToken)
    {
         var userId = User.GetCurrentUserId();
       var sessionId = HttpContext.GetSessionId();

        logger.LogInformation(
            "Clearing cart - UserId: {UserId}, SessionId: {SessionId}",
            userId, sessionId);

        ClearCartCommand command = new(userId, sessionId);
        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Clear cart failed - UserId: {UserId}, Error: {Error}",
                userId, result.Error);

            return result.ToApiResponse("خطا در خالی کردن سبد خرید");
        }

        logger.LogInformation("Cart cleared - UserId: {UserId}", userId);

        return result.ToApiResponse("سبد خرید با موفقیت خالی شد");
    }

    #endregion

    #region Checkout Cart

    // Checkout requires authentication
    [Authorize]
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<Guid>> CheckoutCart(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        logger.LogInformation("Checking out cart - UserId: {UserId}", userId);

        CheckoutCartCommand command = new(userId);
        Result<Guid> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Checkout cart failed - UserId: {UserId}, Error: {Error}",
                userId, result.Error);

            return result.ToApiResponse<Guid>("خطا در ثبت سفارش");
        }

        logger.LogInformation(
            "Cart checked out - UserId: {UserId}, OrderId: {OrderId}",
            userId, result.Value);

        return result.ToApiResponse(
            id => id,
            "سفارش با موفقیت ثبت شد");
    }

    #endregion

    // ======================== QUERIES ========================

    #region Get Cart

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    public async Task<ApiResponse<CartResponse>> GetCart(CancellationToken cancellationToken)
    {
         var userId = User.GetCurrentUserId();
       var sessionId = HttpContext.GetSessionId();

        logger.LogInformation(
            "Getting cart - UserId: {UserId}, SessionId: {SessionId}",
            userId, sessionId);

        Result<CartDto> result = await mediator.Send(
            new GetCartQuery(userId, sessionId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Get cart failed - UserId: {UserId}, Error: {Error}",
                userId, result.Error);

            return result.ToApiResponse(mapper.Map<CartResponse>,"خطا در دریافت سبد خرید");
        }

        logger.LogInformation(
            "Cart retrieved - UserId: {UserId}, ItemsCount: {Count}",
            userId, result.Value.ItemsCount);

        return result.ToApiResponse(
            mapper.Map<CartResponse>,
            "سبد خرید با موفقیت دریافت شد");
    }

    #endregion

    #region Get Cart Item Count

    [HttpGet("count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<ApiResponse<int>> GetCartItemCount(CancellationToken cancellationToken)
    {
         var userId = User.GetCurrentUserId();
       var sessionId = HttpContext.GetSessionId();

        // No userId and no sessionId means empty cart
        if (!userId.HasValue && string.IsNullOrWhiteSpace(sessionId))
            return ApiResponse<int>.Success(0, "تعداد آیتم‌های سبد خرید", 200);

        logger.LogInformation(
            "Getting cart item count - UserId: {UserId}, SessionId: {SessionId}",
            userId, sessionId);

        Result<int> result = await mediator.Send(
            new GetCartItemCountQuery(userId, sessionId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Get cart count failed - UserId: {UserId}, Error: {Error}",
                userId, result.Error);

            return result.ToApiResponse<int>("خطا در دریافت تعداد آیتم‌های سبد خرید");
        }

        logger.LogInformation(
            "Cart count retrieved - UserId: {UserId}, Count: {Count}",
            userId, result.Value);

        return result.ToApiResponse(
            count => count,
            "تعداد آیتم‌های سبد خرید با موفقیت دریافت شد");
    }

    #endregion

    #region Check Course In Cart

    [HttpGet("check/{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ApiResponse<bool>> CheckCourseInCart(
        Guid courseId,
        CancellationToken cancellationToken)
    {
         var userId = User.GetCurrentUserId();
       var sessionId = HttpContext.GetSessionId();

        // No identity at all means course is not in cart
        if (!userId.HasValue && string.IsNullOrWhiteSpace(sessionId))
            return ApiResponse<bool>.Success(false, "بررسی وجود دوره در سبد خرید", 200);

        logger.LogInformation(
            "Checking course in cart - UserId: {UserId}, SessionId: {SessionId}, CourseId: {CourseId}",
            userId, sessionId, courseId);

        Result<bool> result = await mediator.Send(
            new CheckCourseInCartQuery(userId, sessionId, courseId),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Check course in cart failed - UserId: {UserId}, CourseId: {CourseId}, Error: {Error}",
                userId, courseId, result.Error);

            return result.ToApiResponse<bool>("خطا در بررسی وجود دوره در سبد خرید");
        }

        logger.LogInformation(
            "Course in cart check completed - UserId: {UserId}, CourseId: {CourseId}, InCart: {InCart}",
            userId, courseId, result.Value);

        return result.ToApiResponse(
            inCart => inCart,
            "بررسی وجود دوره در سبد خرید با موفقیت انجام شد");
    }

    #endregion
}

#endregion
