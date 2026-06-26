#region Using

using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

#endregion

namespace Vitastic.App.Features.Orders.Commands.UpdateContactInformation;

public sealed class UpdateContactInformationCommandValidator
    : AbstractValidator<UpdateContactInformationCommand>
{
    public UpdateContactInformationCommandValidator()
    {
        #region OrderId

        RuleFor(x => x.OrderId)
            .NotEqual(Guid.Empty)
            .WithMessage("شناسه سفارش معتبر نمی‌باشد.");

        #endregion

        #region PhoneNumber

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("شماره تلفن نمی‌تواند خالی باشد.")
            .MinimumLength(PhoneNumber.MinLength)
            .WithMessage($"شماره تلفن باید حداقل {PhoneNumber.MinLength} کاراکتر باشد.")
            .MaximumLength(PhoneNumber.MaxLength)
            .WithMessage($"شماره تلفن باید حداکثر {PhoneNumber.MaxLength} کاراکتر باشد.");

        #endregion

        #region Billing Address (Required)

        RuleFor(x => x.BillingAddressStreet)
            .NotEmpty()
            .WithMessage("خیابان آدرس صورتحساب نمی‌تواند خالی باشد.")
            .MinimumLength(Address.MinStreetLength)
            .WithMessage($"خیابان آدرس صورتحساب باید حداقل {Address.MinStreetLength} کاراکتر باشد.")
            .MaximumLength(Address.MaxStreetLength)
            .WithMessage($"خیابان آدرس صورتحساب باید حداکثر {Address.MaxStreetLength} کاراکتر باشد.");

        RuleFor(x => x.BillingAddressCity)
            .NotEmpty()
            .WithMessage("شهر آدرس صورتحساب نمی‌تواند خالی باشد.")
            .MinimumLength(Address.MinCityLength)
            .WithMessage($"شهر آدرس صورتحساب باید حداقل {Address.MinCityLength} کاراکتر باشد.")
            .MaximumLength(Address.MaxCityLength)
            .WithMessage($"شهر آدرس صورتحساب باید حداکثر {Address.MaxCityLength} کاراکتر باشد.");

        RuleFor(x => x.BillingAddressState)
            .NotEmpty()
            .WithMessage("استان آدرس صورتحساب نمی‌تواند خالی باشد.")
            .MinimumLength(Address.MinStateLength)
            .WithMessage($"استان آدرس صورتحساب باید حداقل {Address.MinStateLength} کاراکتر باشد.")
            .MaximumLength(Address.MaxStateLength)
            .WithMessage($"استان آدرس صورتحساب باید حداکثر {Address.MaxStateLength} کاراکتر باشد.");

        RuleFor(x => x.BillingAddressPostalCode)
            .NotEmpty()
            .WithMessage("کد پستی آدرس صورتحساب نمی‌تواند خالی باشد.")
            .Length(Address.PostalCodeLength)
            .WithMessage($"کد پستی آدرس صورتحساب باید {Address.PostalCodeLength} کاراکتر باشد.");

        RuleFor(x => x.BillingAddressCountry)
            .NotEmpty()
            .WithMessage("کشور آدرس صورتحساب نمی‌تواند خالی باشد.")
            .MinimumLength(Address.MinCountryLength)
            .WithMessage($"کشور آدرس صورتحساب باید حداقل {Address.MinCountryLength} کاراکتر باشد.")
            .MaximumLength(Address.MaxCountryLength)
            .WithMessage($"کشور آدرس صورتحساب باید حداکثر {Address.MaxCountryLength} کاراکتر باشد.");

        #endregion

        #region Shipping Address (Optional — validated only when provided)

        When(x => !string.IsNullOrWhiteSpace(x.ShippingAddressStreet), () =>
        {
            RuleFor(x => x.ShippingAddressStreet)
                .MinimumLength(Address.MinStreetLength)
                .WithMessage($"خیابان آدرس ارسال باید حداقل {Address.MinStreetLength} کاراکتر باشد.")
                .MaximumLength(Address.MaxStreetLength)
                .WithMessage($"خیابان آدرس ارسال باید حداکثر {Address.MaxStreetLength} کاراکتر باشد.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ShippingAddressCity), () =>
        {
            RuleFor(x => x.ShippingAddressCity)
                .MinimumLength(Address.MinCityLength)
                .WithMessage($"شهر آدرس ارسال باید حداقل {Address.MinCityLength} کاراکتر باشد.")
                .MaximumLength(Address.MaxCityLength)
                .WithMessage($"شهر آدرس ارسال باید حداکثر {Address.MaxCityLength} کاراکتر باشد.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ShippingAddressState), () =>
        {
            RuleFor(x => x.ShippingAddressState)
                .MinimumLength(Address.MinStateLength)
                .WithMessage($"استان آدرس ارسال باید حداقل {Address.MinStateLength} کاراکتر باشد.")
                .MaximumLength(Address.MaxStateLength)
                .WithMessage($"استان آدرس ارسال باید حداکثر {Address.MaxStateLength} کاراکتر باشد.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ShippingAddressPostalCode), () =>
        {
            RuleFor(x => x.ShippingAddressPostalCode)
                .Length(Address.PostalCodeLength)
                .WithMessage($"کد پستی آدرس ارسال باید {Address.PostalCodeLength} کاراکتر باشد.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ShippingAddressCountry), () =>
        {
            RuleFor(x => x.ShippingAddressCountry)
                .MinimumLength(Address.MinCountryLength)
                .WithMessage($"کشور آدرس ارسال باید حداقل {Address.MinCountryLength} کاراکتر باشد.")
                .MaximumLength(Address.MaxCountryLength)
                .WithMessage($"کشور آدرس ارسال باید حداکثر {Address.MaxCountryLength} کاراکتر باشد.");
        });

        #endregion
    }
}
