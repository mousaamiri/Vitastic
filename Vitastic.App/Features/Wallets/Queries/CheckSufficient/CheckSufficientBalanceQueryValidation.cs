using FluentValidation;

namespace Vitastic.App.Features.Wallets.Queries.CheckSufficient
{
    public sealed class CheckSufficientBalanceQueryValidation : AbstractValidator<CheckSufficientBalanceQuery>
    {
        public CheckSufficientBalanceQueryValidation()
        {
            RuleFor(x => x.WalletId).NotEqual(Guid.Empty).WithMessage("شناسه کیف پول معتبر نمی باشد.");
            RuleFor(x => x.Amount).GreaterThan(0)
                .WithMessage("مقدار باید بزرگتر از صفر باشد.");
        }
    }
}