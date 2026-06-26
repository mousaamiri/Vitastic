using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;


namespace Vitastic.App.Features.Orders.Commands.UpdateContactInformation;

public sealed class UpdateContactInformationCommandHandler(
    IOrderRepository orderRepository) : ICommandHandler<UpdateContactInformationCommand>
{
    public async Task<Result> Handle(
        UpdateContactInformationCommand request,
        CancellationToken cancellationToken)
    {
        #region Find Order

        var orderIdResult = OrderId.CreateFrom(request.OrderId);
        if (orderIdResult.IsFailure)
            return orderIdResult.Error;

        var order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
        if (order is null)
            return Error.NotFound(
                "UpdateContactInformation.OrderNotFound",
                "سفارشی با این شناسه یافت نشد.");

        #endregion

        #region Build Value Objects

        // Build PhoneNumber Value Object
        var phoneResult = PhoneNumber.Create(request.PhoneNumber);
        if (phoneResult.IsFailure)
            return phoneResult.Error;

        // Build Billing Address Value Object (required)
        var billingResult = Address.Create(
            request.BillingAddressStreet,
            request.BillingAddressCity,
            request.BillingAddressState,
            request.BillingAddressPostalCode,
            request.BillingAddressCountry);

        if (billingResult.IsFailure)
            return billingResult.Error;

        // Build Shipping Address Value Object (optional — only if any field is provided)
        Address? shippingAddress = null;
        bool hasShippingData = !string.IsNullOrWhiteSpace(request.ShippingAddressStreet)
                            || !string.IsNullOrWhiteSpace(request.ShippingAddressCity)
                            || !string.IsNullOrWhiteSpace(request.ShippingAddressState)
                            || !string.IsNullOrWhiteSpace(request.ShippingAddressPostalCode)
                            || !string.IsNullOrWhiteSpace(request.ShippingAddressCountry);

        if (hasShippingData)
        {
            // When shipping is partially provided, all fields must be present
            if (string.IsNullOrWhiteSpace(request.ShippingAddressStreet)
                || string.IsNullOrWhiteSpace(request.ShippingAddressCity)
                || string.IsNullOrWhiteSpace(request.ShippingAddressState)
                || string.IsNullOrWhiteSpace(request.ShippingAddressPostalCode)
                || string.IsNullOrWhiteSpace(request.ShippingAddressCountry))
            {
                return Error.Validation(
                    "UpdateContactInformation.IncompleteShippingAddress",
                    "اگر آدرس ارسال وارد می‌شود، تمام فیلدهای آن الزامی هستند.");
            }

            var shippingResult = Address.Create(
                request.ShippingAddressStreet!,
                request.ShippingAddressCity!,
                request.ShippingAddressState!,
                request.ShippingAddressPostalCode!,
                request.ShippingAddressCountry!);

            if (shippingResult.IsFailure)
                return shippingResult.Error;

            shippingAddress = shippingResult.Value;
        }

        #endregion

        #region Update Order & Persist

        // Delegate to Domain — all business rules enforced inside Order aggregate
        var updateResult = order.UpdateContactInformation(
            phoneResult.Value,
            billingResult.Value,
            shippingAddress);

        if (updateResult.IsFailure)
            return updateResult.Error;

        await orderRepository.UpdateAsync(order, cancellationToken);

        #endregion

        return Result.Success();
    }
}
