using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.UserPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Order;
namespace Vitastic.Web.Areas.UserPanel.Controllers;

public class UserOrdersController(IOrderService orderService) : UserController
{
    #region Index

    [HttpGet]
    public async Task<IActionResult> Index(
        OrderStatusDto? status = OrderStatusDto.Completed,
        int pageNumber = 1,
        int pageSize = 10)
    {
        PaginatedApiResponse<OrderDto> result = await orderService.GetMyOrders(status, pageNumber, pageSize);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Message ?? "خطا در دریافت سفارشات";
            return View(new PaginatedData<OrderDto>
            {
                Items = new List<OrderDto>(),
                TotalCount = 0,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        ViewBag.CurrentStatus = status;
        return View(result.Data);
    }

    #endregion
    #region Order Details

    [HttpGet("Details/{orderId:guid}")]
    public async Task<IActionResult> OrderDetails(Guid orderId)
    {
        ApiResponse<OrderDetailApiDto> result = await orderService.GetOrderDetails(orderId);
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.Message ?? "خطا در دریافت جزئیات سفارش" });
        }
        return Json(result.Data);
    }

    #endregion


}
