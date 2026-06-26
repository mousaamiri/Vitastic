using FluentValidation;

namespace Vitastic.App.Features.Wallets.Queries.GetByUserId
{
    public sealed class GetUserWalletQueryValidation : AbstractValidator<GetUserWalletQuery>
    {
        public GetUserWalletQueryValidation()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("شناسه کاربر معتبر نمی باشد.");
        }
    }
}