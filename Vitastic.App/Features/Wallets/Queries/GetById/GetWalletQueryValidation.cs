using FluentValidation;

namespace Vitastic.App.Features.Wallets.Queries.GetById
{
    public sealed class GetWalletQueryValidation : AbstractValidator<GetWalletQuery>
    {
        public GetWalletQueryValidation()
        {
            RuleFor(x => x.WalletId).NotEqual(Guid.Empty).WithMessage("شناسه کیف پول معتبر نمی باشد.");


        }
    }
}
