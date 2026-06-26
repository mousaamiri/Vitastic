using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Courses.Requests;
using Vitastic.Api.Features.Orders.Requests;
using Vitastic.Api.Features.Orders.Responses;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Orders.Commands.AddAdminNote;
using Vitastic.App.Features.Orders.Commands.AddCustomerInfo;
using Vitastic.App.Features.Orders.Commands.AddCustomerNote;
using Vitastic.App.Features.Orders.Commands.AddItemToOrder;
using Vitastic.App.Features.Orders.Commands.ApplyDiscount;
using Vitastic.App.Features.Orders.Commands.CancelOrder;
using Vitastic.App.Features.Orders.Commands.ChangeStatus;
using Vitastic.App.Features.Orders.Commands.ClearOrderItems;
using Vitastic.App.Features.Orders.Commands.CompleteOrder;
using Vitastic.App.Features.Orders.Commands.Create;
using Vitastic.App.Features.Orders.Commands.ProcessPayment;
using Vitastic.App.Features.Orders.Commands.RefundOrder;
using Vitastic.App.Features.Orders.Commands.RemoveDiscount;
using Vitastic.App.Features.Orders.Commands.RemoveItemFromOrder;
using Vitastic.App.Features.Orders.Commands.SetShippingAmount;
using Vitastic.App.Features.Orders.Commands.SetTaxAmount;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.App.Features.Orders.Queries.GetById;
using Vitastic.App.Features.Orders.Queries.GetOrderItems;
using Vitastic.App.Features.Orders.Queries.GetUserOrders;
using Vitastic.App.Features.Orders.Queries.List;
using Vitastic.App.Features.Orders.Queries.Search;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Features.Orders;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class OrdersController(
    IMediator mediator,
    IMapper mapper,
    ILogger<OrdersController> logger) : ControllerBase
{
    // ══════════════════════════════════════════════════════
    //                      COMMANDS
    // ══════════════════════════════════════════════════════

    #region Create Order

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse<Guid>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating order for user {UserId}", request.UserId);

        var command = mapper.Map<CreateOrderCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Create order failed - UserId: {UserId}, Error: {ErrorCode}",
                request.UserId, result.Error.Code);

            return result.ToApiResponse(id=>id, "خطا در ایجاد سفارش");
        }

        logger.LogInformation("Order created - OrderId: {OrderId}, UserId: {UserId}",
            result.Value, request.UserId);

        return result.ToApiResponse( id=>id,"سفارش با موفقیت ایجاد شد");
    }

    #endregion

    #region Add Item To Order

    [HttpPost("{orderId:guid}/items")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse<Guid>> AddItemToOrder(
        Guid orderId,
        [FromBody] AddItemToOrderRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding course {CourseId} to order {OrderId}",
            request.CourseId, orderId);

        var command = new AddItemToOrderCommand(orderId, request.CourseId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Add item failed - CourseId: {CourseId}, OrderId: {OrderId}, Error: {ErrorCode}",
                request.CourseId, orderId, result.Error.Code);

            return result.ToApiResponse<Guid>( "خطا در اضافه کردن آیتم به سفارش");
        }

        logger.LogInformation("Item added - ItemId: {ItemId}, OrderId: {OrderId}",
            result.Value, orderId);

        return result.ToApiResponse<Guid>( "آیتم با موفقیت به سفارش اضافه شد");
    }

    #endregion

    #region Add Admin Note

    [HttpPatch("{orderId:guid}/admin-notes")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> AddAdminNote(
        Guid orderId,
        [FromBody] AddAdminNoteRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding admin note to order {OrderId}", orderId);

        var command = new AddAdminNoteCommand(orderId, request.Note);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Add admin note failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در افزودن یادداشت ادمین");
        }

        logger.LogInformation("Admin note added - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("یادداشت ادمین با موفقیت اضافه شد");
    }

    #endregion
    #region Change Order Status
    [Authorize(Policy = "AdminOnly")]
    [HttpPatch("{orderId:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> ChangeStatus(
        Guid orderId,
        [FromBody] ChangeOrderStatusByAdminRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Changing order status - OrderId: {OrderId}, NewStatus: {Status}",
            orderId,
            request.Status);

        var command = new ChangeOrderStatusCommand(
            orderId,
            request.Status,
            request.AdminNote);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Status change failed - OrderId: {OrderId}, Status: {Status}, Error: {ErrorCode}",
                orderId,
                request.Status,
                result.Error.Code);

            return result.ToApiResponse("خطا در تغییر وضعیت سفارش");
        }

        logger.LogInformation(
            "Order status changed successfully - OrderId: {OrderId}, NewStatus: {Status}",
            orderId,
            request.Status);

        return result.ToApiResponse("وضعیت سفارش با موفقیت تغییر یافت");
    }
    #endregion


    #region Add Customer Note

    [HttpPatch("{orderId:guid}/customer-notes")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> AddCustomerNote(
        Guid orderId,
        [FromBody] AddCustomerNoteRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding customer note to order {OrderId}", orderId);

        var command = new AddCustomerNoteCommand(orderId, request.Note);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Add customer note failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در افزودن یادداشت مشتری");
        }

        logger.LogInformation("Customer note added - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("یادداشت مشتری با موفقیت اضافه شد");
    }

    #endregion
    #region Add Customer Note

    [HttpPatch("{orderId:guid}/customer-information")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> AddCustomerInformation(
        Guid orderId,
        string customerFullName,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding customer information to order {OrderId}", orderId);

        var command = new AddCustomerInformationCommand(orderId, customerFullName);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Add customer information failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در بروز رسانی اطلاعات مشتری");
        }

        logger.LogInformation("Customer information added - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("اطلاعات مشتری با موفقیت بروز شد");
    }

    #endregion
    #region Apply Discount

    [HttpPatch("{orderId:guid}/discount")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> ApplyDiscount(
        Guid orderId,
        [FromBody] ApplyDiscountRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Applying discount {DiscountId} to order {OrderId}",
            request.DiscountId, orderId);

        var command = new ApplyDiscountCommand(orderId, request.DiscountId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Apply discount failed - DiscountId: {DiscountId}, OrderId: {OrderId}, Error: {ErrorCode}",
                request.DiscountId, orderId, result.Error.Code);

            return result.ToApiResponse("خطا در اعمال تخفیف");
        }

        logger.LogInformation("Discount applied - DiscountId: {DiscountId}, OrderId: {OrderId}",
            request.DiscountId, orderId);

        return result.ToApiResponse("تخفیف با موفقیت اعمال شد");
    }

    #endregion

    #region Cancel Order

    [HttpPatch("{orderId:guid}/status/cancelled")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> CancelOrder(
        Guid orderId,
        [FromBody] CancelOrderRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Cancelling order {OrderId}, reason: {Reason}",
            orderId, request.CancelReason ?? "N/A");

        var command = new CancelOrderCommand(orderId, request.CancelReason);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Cancel order failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در لغو سفارش");
        }

        logger.LogInformation("Order cancelled - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("سفارش با موفقیت لغو شد");
    }

    #endregion

    #region Complete Order

    [HttpPatch("{orderId:guid}/status/completed")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> CompleteOrder(
        Guid orderId,
        [FromBody] CompleteOrderRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Completing order {OrderId}", orderId);

        var command = new CompleteOrderCommand(orderId,request.TransactionId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Complete order failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در تکمیل سفارش");
        }

        logger.LogInformation("Order completed - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("سفارش با موفقیت تکمیل شد");
    }

    #endregion

    #region Process Payment

    [HttpPatch("{orderId:guid}/payment")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> ProcessPayment(
        Guid orderId,
        [FromBody] ProcessPaymentRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Processing payment for order {OrderId}, transaction {TransactionId}, method {Method}",
            orderId, request.TransactionId, request.PaymentMethodResponse);

        var command = new ProcessPaymentCommand(
            orderId,
            request.TransactionId,
            (PaymentMethodDto)request.PaymentMethodResponse);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Payment failed - OrderId: {OrderId}, TransactionId: {TransactionId}, Error: {ErrorCode}",
                orderId, request.TransactionId, result.Error.Code);

            return result.ToApiResponse("خطا در پردازش پرداخت");
        }

        logger.LogInformation(
            "Payment processed - OrderId: {OrderId}, TransactionId: {TransactionId}",
            orderId, request.TransactionId);

        return result.ToApiResponse("پرداخت با موفقیت پردازش شد");
    }

    #endregion

    #region Refund Order

    [HttpPatch("{orderId:guid}/refund")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> RefundOrder(
        Guid orderId,
        [FromBody] RefundOrderRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Refunding order {OrderId}, reason: {Reason}",
            orderId, request.RefundReason ?? "N/A");

        var command = new RefundOrderCommand(orderId, request.RefundReason);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Refund order failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در بازپرداخت سفارش");
        }

        logger.LogInformation("Order refunded - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("سفارش با موفقیت بازپرداخت شد");
    }

    #endregion

    #region Remove Discount

    [HttpDelete("{orderId:guid}/discount")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> RemoveDiscount(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing discount from order {OrderId}", orderId);

        var command = new RemoveDiscountCommand(orderId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Remove discount failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در حذف تخفیف");
        }

        logger.LogInformation("Discount removed - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("تخفیف با موفقیت حذف شد");
    }

    #endregion

    #region Remove Item From Order

    [HttpDelete("{orderId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> RemoveItemFromOrder(
        Guid orderId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing item {ItemId} from order {OrderId}",
            itemId, orderId);

        var command = new RemoveItemFromOrderCommand(orderId, itemId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Remove item failed - ItemId: {ItemId}, OrderId: {OrderId}, Error: {ErrorCode}",
                itemId, orderId, result.Error.Code);

            return result.ToApiResponse("خطا در حذف آیتم از سفارش");
        }

        logger.LogInformation("Item removed - ItemId: {ItemId}, OrderId: {OrderId}",
            itemId, orderId);

        return result.ToApiResponse("آیتم با موفقیت از سفارش حذف شد");
    }

    #endregion


    // ══════════════════════════════════════════════════════
    //                      QUERIES
    // ══════════════════════════════════════════════════════

    #region Set Shipping Amount

    [HttpPatch("{orderId:guid}/shipping")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> SetShippingAmount(
        Guid orderId,
        [FromBody] SetShippingAmountRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting shipping amount {Amount} for order {OrderId}",
            request.ShippingAmount, orderId);

        var command = new SetShippingAmountCommand(orderId, request.ShippingAmount);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Set shipping amount failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در تنظیم هزینه ارسال");
        }

        logger.LogInformation("Shipping amount set - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("هزینه ارسال با موفقیت تنظیم شد");
    }

    #endregion

    #region Set Tax Amount

    [HttpPatch("{orderId:guid}/tax")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> SetTaxAmount(
        Guid orderId,
        [FromBody] SetTaxAmountRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting tax amount {Amount} for order {OrderId}",
            request.TaxAmount, orderId);

        var command = new SetTaxAmountCommand(orderId, request.TaxAmount);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Set tax amount failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در تنظیم مالیات");
        }

        logger.LogInformation("Tax amount set - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("مالیات با موفقیت تنظیم شد");
    }

    #endregion

    #region Clear Order Items

    [HttpDelete("{orderId:guid}/items")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> ClearOrderItems(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Clearing all items from order {OrderId}", orderId);

        var command = new ClearOrderItemsCommand(orderId);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Clear order items failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse("خطا در حذف آیتم‌های سفارش");
        }

        logger.LogInformation("All items cleared - OrderId: {OrderId}", orderId);

        return result.ToApiResponse("تمام آیتم‌ها با موفقیت حذف شدند");
    }

    #endregion

    #region Get Order By Id

    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<OrderDetailResponse>> GetOrderById(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting order by id {OrderId}", orderId);


        var result = await mediator.Send(new GetOrderByIdQuery(orderId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Get order failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse(mapper.Map<OrderDetailResponse>, "خطا در دریافت سفارش");
        }


        logger.LogInformation("Order retrieved - OrderId: {OrderId}", orderId);

        return result.ToApiResponse(mapper.Map<OrderDetailResponse>, "سفارش با موفقیت دریافت شد");
    }

    #endregion

    #region Get Order Items

    [HttpGet("{orderId:guid}/items")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrderItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<IReadOnlyList<OrderItemResponse>>> GetOrderItems(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting order items for order {OrderId}", orderId);

        Result<IReadOnlyList<OrderItemDto>> result = await mediator.Send(
            new GetOrderItemsQuery(orderId),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Get order items failed - OrderId: {OrderId}, Error: {ErrorCode}",
                orderId, result.Error.Code);

            return result.ToApiResponse<IReadOnlyList<OrderItemResponse>>( "خطا در دریافت آیتم‌های سفارش");
        }


        logger.LogInformation("Order items retrieved - OrderId: {OrderId}", orderId);

        return result
            .ToApiResponse(mapper.Map<IReadOnlyList<OrderItemResponse>>, "آیتم‌های سفارش با موفقیت دریافت شدند");
    }

    #endregion

    #region List Orders

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<OrderResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<OrderResponse>>> ListOrders(
        [FromQuery] OrderStatusDto? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Listing orders - Page: {PageNumber}, Size: {PageSize}, Status: {Status}",
            pageNumber, pageSize, status);

        var result = await mediator.Send(
            new GetPagedOrdersQuery(pageNumber, pageSize, status),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("List orders failed - Error: {ErrorCode}",
                result.Error.Code);

            return result.ToApiResponse<PaginatedResponse<OrderResponse>>("خطا در دریافت لیست سفارشات");
        }


        logger.LogInformation("Orders listed successfully");

        return result
            .ToApiResponse(mapper.Map<PaginatedResponse<OrderResponse>>, "لیست سفارشات با موفقیت دریافت شد");
    }

    #endregion
    #region User Orders
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<OrderResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<OrderResponse>>> GetUserOrders(
        [FromQuery] OrderStatusDto? status ,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetCurrentUserId();
        if (userId is null)
        {
            return ApiResponse<PaginatedResponse<OrderResponse>>
                .Fail("خطا در دریافت سفارش های کاربر، کاربر لاگین نشده است.",ErrorType.Unauthorized);
        }
        logger.LogInformation("درخواست لیست سفارشات کاربر - شناسه کاربر: {UserId}, صفحه: {PageNumber}, اندازه: {PageSize}, وضعیت: {Status}",
            userId, pageNumber, pageSize, status);
        var result = await mediator.Send(
            new GetUserOrdersQuery(userId.Value, pageNumber, pageSize, status),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("دریافت لیست سفارشات با شکست مواجه شد - کد خطا: {ErrorCode}",
                result.Error.Code);

            return result.ToApiResponse<PaginatedResponse<OrderResponse>>("خطا در دریافت لیست سفارشات");
        }

        logger.LogInformation("لیست سفارشات کاربر با موفقیت بازگردانده شد");

        return result.ToApiResponse(
            mapper.Map<PaginatedResponse<OrderResponse>>,
            "لیست سفارشات با موفقیت دریافت شد");
    }

    #endregion


    #region Search Orders

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<OrderResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<OrderResponse>>> SearchOrders(
        [FromQuery] string searchTerm,
        [FromQuery] OrderStatusDto? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Searching orders - Term: {SearchTerm}, Page: {PageNumber}, Size: {PageSize}, Status: {Status}",
            searchTerm, pageNumber, pageSize, status);

        var result = await mediator.Send(
            new SearchOrdersQuery(searchTerm, pageNumber, pageSize, status),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Search orders failed - Term: {SearchTerm}, Error: {ErrorCode}",
                searchTerm, result.Error.Code);

            return result.ToApiResponse<PaginatedResponse<OrderResponse>>( "خطا در جستجوی سفارشات");
        }


        logger.LogInformation("Search orders completed - Term: {SearchTerm}", searchTerm);

        return result
            .ToApiResponse(mapper.Map<PaginatedResponse<OrderResponse>>, "نتایج جستجو با موفقیت دریافت شد");
    }

    #endregion
}
