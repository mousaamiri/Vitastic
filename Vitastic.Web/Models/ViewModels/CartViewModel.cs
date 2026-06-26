using Vitastic.Web.Models.DTOs.Cart;

namespace Vitastic.Web.Models.ViewModels;

public sealed class CartViewModel
{
    public CartDto Cart { get; init; } = null!;
    public CartSummaryDto Summary { get; init; } = null!;
    public bool IsEmpty => Cart?.ItemsCount == 0;
    public bool HasItems => !IsEmpty;
}
