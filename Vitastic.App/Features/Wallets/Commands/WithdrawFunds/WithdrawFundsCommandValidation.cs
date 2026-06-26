using FluentValidation;

namespace Vitastic.App.Features.Wallets.Commands.WithdrawFunds
{
    public sealed class WithdrawFundsCommandValidation : AbstractValidator<WithdrawFundsCommand>
    {
        public WithdrawFundsCommandValidation()
        {
            RuleFor(c => c.WalletId).NotEqual(Guid.Empty).WithMessage("شناسه کیف پول نمی تواند خالی باشد.");
            RuleFor(c => c.Amount).GreaterThan(0).WithMessage("مبلغ باید بزرگتر از صفر باشد.");
        }
    }
}