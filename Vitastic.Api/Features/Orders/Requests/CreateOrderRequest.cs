using Vitastic.Api.Features.Orders.Responses;

namespace Vitastic.Api.Features.Orders.Requests;


public sealed record AddAdminNoteRequest(
    string Note);

public sealed record AddCustomerNoteRequest(
    string Note);

public sealed record AddItemToOrderRequest(
    Guid CourseId);

public sealed record ApplyDiscountRequest(
    Guid DiscountId);

public sealed record CancelOrderRequest(
    string? CancelReason = null);

public sealed record CreateOrderRequest(Guid UserId);

public sealed record ProcessPaymentRequest(
    Guid TransactionId,
    PaymentMethodResponse PaymentMethodResponse);
public sealed record CompleteOrderRequest(
    Guid TransactionId);

public sealed record RefundOrderRequest(
    string? RefundReason = null);

public sealed record RemoveItemFromOrderRequest(
    Guid OrderItemId);

public sealed record SetShippingAmountRequest(
    decimal ShippingAmount);

public sealed record SetTaxAmountRequest(
    decimal TaxAmount);

public sealed record UpdateContactInformationRequest(
    string PhoneNumber,
    string BillingAddressStreet,
    string BillingAddressCity,
    string BillingAddressState,
    string BillingAddressCountry,
    string BillingAddressPostalCode,
    string? ShippingAddressStreet = null,
    string? ShippingAddressCity = null,
    string? ShippingAddressState = null,
    string? ShippingAddressPostalCode = null,
    string? ShippingAddressCountry = null);
