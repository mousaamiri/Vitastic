using FluentValidation;

namespace Vitastic.App.Features.Wallets.Queries.GetUserTransactions
{
    public sealed class GetUserTransactionsQueryValidation : AbstractValidator<GetUserTransactionsQuery>
    {
        public GetUserTransactionsQueryValidation()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("شناسه کاربر معتبر نمی باشد.");
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("شماره صفحه باید بیشتر از 0 باشد");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("تعداد آیتم‌ها باید بین 1 و 100 باشد");
        }
    }
}