using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.AdminPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Order;

namespace Vitastic.Web.Areas.AdminPanel.Controllers;

public class OrderManagerController(IOrderService orderService):AdminController
{
    #region Index

    public async Task<IActionResult> Index(OrderStatusDto status=OrderStatusDto.Pending,int pageNumber = 1,int pageSize = 10,CancellationToken ct = default)
    {
        PaginatedApiResponse<OrderDto> response = await orderService.GetOrdersList(status,pageNumber, pageSize, ct);
        if (!response.IsSuccess)
            TempData["ErrorMessage"] = response.Errors.Count > 0 ? response.Errors[0] : response.Message;
        return View(response.Data);
    }

    #endregion

    #region Details

    [HttpGet("Details/{orderId}")]
    public async Task<IActionResult> Details(Guid orderId,CancellationToken ct=default)
    {
        ApiResponse<OrderDetailApiDto> response = await orderService.GetOrderDetails(orderId, ct);
        if(!response.IsSuccess)
            TempData["ErrorMessage"] = response.Errors.Count>0? response.Errors.First() : response.Message;
        return View("_OrderDetails",response.Data);
    }

    #endregion
    #region ChangeStatus

    [HttpPost("ChangeStatus/{orderId}")]
    public async Task<IActionResult> ChangeStatus(Guid orderId, [FromBody] ChangeOrderStatusByAdminRequest request, CancellationToken ct = default)
    {
        ApiResponse response = await orderService.ChangeStatus(orderId, request, ct);
        if (!response.IsSuccess)
            return BadRequest(new { message = response.Errors.Count > 0 ? response.Errors.First() : response.Message });

        return Ok(new { message = response.Message });
    }

    #endregion
}
