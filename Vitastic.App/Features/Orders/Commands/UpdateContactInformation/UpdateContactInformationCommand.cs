using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.UpdateContactInformation;

public sealed record UpdateContactInformationCommand(
    Guid OrderId,
    string PhoneNumber,

    // Billing address (required)
    string BillingAddressStreet,
    string BillingAddressCity,
    string BillingAddressState,
    string BillingAddressCountry,
    string BillingAddressPostalCode,

    // Shipping address (optional)
    string? ShippingAddressStreet = null,
    string? ShippingAddressCity = null,
    string? ShippingAddressState = null,
    string? ShippingAddressPostalCode = null,
    string? ShippingAddressCountry = null) : ICommand;
