using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Wallets.Commands.Create
{
    public sealed class CreateWalletCommandValidation : AbstractValidator<CreateWalletCommand>
    {
        public CreateWalletCommandValidation()
        {
            RuleFor(command => command.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage("شناسه کاربر نمیتواند خالی باشد.");
            RuleFor(command => command.CurrencyCode)
                .Length(Currency.CodeLength)
                .WithMessage($"کد ارز حتما باید دقیقا {Currency.CodeLength} کاراکتر باشد.");
        }

    }
}
