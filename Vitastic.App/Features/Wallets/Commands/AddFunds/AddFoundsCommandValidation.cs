using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Wallets.Commands.AddFunds
{
    public sealed class AddFoundsCommandValidation : AbstractValidator<AddFundsCommand>
    {
        public AddFoundsCommandValidation()
        {
            RuleFor(command => command.Amount)
                .GreaterThan(0).WithMessage("مقدار باید بیشتر از صفر باشد.");
            RuleFor(command => command.WalletId)
                .NotEqual(Guid.Empty).WithMessage("شناسه کیف پول نمی‌تواند خالی باشد.");
            When(command => !string.IsNullOrEmpty(command.Description), () =>
            {
                RuleFor(command => command.Description)
                    .MaximumLength(Description.MaxLength).WithMessage($"توضیحات نمی‌تواند بیشتر از {Description.MaxLength} کاراکتر باشد.");
            });
        }

    }
}
