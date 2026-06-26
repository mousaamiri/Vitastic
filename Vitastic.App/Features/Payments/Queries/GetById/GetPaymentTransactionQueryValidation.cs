using FluentValidation;

namespace Vitastic.App.Features.Payments.Queries.GetById;

public sealed class GetPaymentTransactionQueryValidation : AbstractValidator<GetPaymentTransactionQuery>
{
    public GetPaymentTransactionQueryValidation()
    {
        RuleFor(x => x.TransactionId).NotEqual(Guid.Empty).WithMessage("شناسه تراکنش معتبر نمی باشد.");
    }
}