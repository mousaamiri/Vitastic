using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Cart;

namespace Vitastic.Web.Infrastructure.Services;

public interface ICartService
{
    Task<ApiResponse<Guid>> AddCartItem(AddCartItemRequest request);
    Task<ApiResponse> RemoveCartItem(Guid cartItemId);
    Task<ApiResponse> ClearCart();
    Task<ApiResponse<Guid>> CheckoutCart();
    Task<ApiResponse<CartDto>> GetCart();
    Task<ApiResponse<int>> GetCartItemCount();
    Task<ApiResponse<bool>> CheckCourseInCart(Guid courseId);

}

internal class CartService(IApiClient apiClient) : ICartService
{
    #region Constants
    private const string Endpoint = "Carts";
    #endregion

    #region Commands

    public async Task<ApiResponse<Guid>> AddCartItem(AddCartItemRequest request)
        => await apiClient.PostAsync<Guid>($"{Endpoint}/items", request);

    public async Task<ApiResponse> RemoveCartItem(Guid cartItemId)
        => await apiClient.DeleteAsync($"{Endpoint}/items/{cartItemId}");

    public async Task<ApiResponse> ClearCart()
        => await apiClient.DeleteAsync(Endpoint);

    public async Task<ApiResponse<Guid>> CheckoutCart()
        => await apiClient.PostAsync<Guid>($"{Endpoint}/checkout");

    #endregion

    #region Queries

    public async Task<ApiResponse<CartDto>> GetCart()
        => await apiClient.GetAsync<CartDto>(Endpoint);

    public async Task<ApiResponse<int>> GetCartItemCount()
        => await apiClient.GetAsync<int>($"{Endpoint}/count");

    public async Task<ApiResponse<bool>> CheckCourseInCart(Guid courseId)
        => await apiClient.GetAsync<bool>($"{Endpoint}/check/{courseId}");

    #endregion
}
