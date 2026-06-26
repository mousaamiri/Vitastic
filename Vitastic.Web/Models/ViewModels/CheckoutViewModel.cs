using Vitastic.Web.Models.DTOs.Order;

namespace Vitastic.Web.Models.ViewModels;


public class CheckoutViewModel
{
    internal OrderDetailApiDto Order { get; set; }
    // Payment
    public List<PaymentMethodOption> PaymentMethods { get; set; } = [];
    public int? SelectedPaymentMethodId { get; set; }

    // Note
    public string? CustomerNote { get; set; }
}
