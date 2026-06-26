namespace Vitastic.Web.Models.DTOs.Checkout;

public class CheckoutSubmitDto
{
    public Guid OrderId { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int PaymentMethodId { get; set; }
    public string? CustomerNote { get; set; }
    public decimal FinalAmount { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string CallbackUrl { get; set; } = string.Empty;
}
public sealed record AddCustomerNoteRequest(string Note);
