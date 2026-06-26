using FluentValidation;

namespace Vitastic.App.Features.Wallets.Queries.GetBalance
{
    public sealed class GetWalletBalanceQueryValidation : AbstractValidator<GetWalletBalanceQuery>
    {
        public GetWalletBalanceQueryValidation()
        {
            RuleFor(x => x.WalletId).NotEqual(Guid.Empty).WithMessage("شناسه کیف پول معتبر نمی باشد.");
        }
    }
}