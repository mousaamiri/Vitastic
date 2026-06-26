using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Order;

namespace Vitastic.Web.Infrastructure.Services;

public interface IOrderService
{
    Task<PaginatedApiResponse<OrderDto>> GetMyOrders(
        OrderStatusDto? status = OrderStatusDto.Completed,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    Task<PaginatedApiResponse<OrderDto>> GetOrdersList(
        OrderStatusDto? status = OrderStatusDto.Completed,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    Task<ApiResponse<OrderDetailApiDto>> GetOrderDetails(Guid orderId, CancellationToken ct=default);
    Task<ApiResponse> ChangeStatus(Guid orderId, ChangeOrderStatusByAdminRequest request, CancellationToken ct);
}

internal class OrderService(IApiClient apiClient) : IOrderService
{
    #region Get my orders

    public async Task<PaginatedApiResponse<OrderDto>> GetMyOrders(
        OrderStatusDto? status,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    => await apiClient.GetPaginatedAsync<OrderDto>($"orders/my",
        new{ status, pageNumber, pageSize}, cancellationToken);

    public async Task<PaginatedApiResponse<OrderDto>> GetOrdersList(OrderStatusDto? status, int pageNumber = 1, int pageSize = 10,
        CancellationToken cancellationToken = default)
        => await apiClient.GetPaginatedAsync<OrderDto>($"orders",
    new{ status, pageNumber, pageSize }, cancellationToken);
    #endregion

    #region Get order details item
    public async Task<ApiResponse<OrderDetailApiDto>> GetOrderDetails(Guid orderId, CancellationToken ct = default)
        => await apiClient.GetAsync<OrderDetailApiDto>($"orders/{orderId}", ct);

    public async Task<ApiResponse> ChangeStatus(Guid orderId, ChangeOrderStatusByAdminRequest request, CancellationToken ct)
        => await apiClient.PatchAsync<OrderDetailApiDto>($"orders/{orderId}/status",request, ct);


    #endregion
}
