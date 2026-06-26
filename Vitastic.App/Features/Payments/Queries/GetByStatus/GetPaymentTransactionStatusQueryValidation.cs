using FluentValidation;

namespace Vitastic.App.Features.Payments.Queries.GetByStatus;

public sealed class GetPaymentTransactionStatusQueryValidation : AbstractValidator<GetPaymentTransactionStatusQuery>
{
    public GetPaymentTransactionStatusQueryValidation()
    {
        RuleFor(x => x.TransactionId).NotEqual(Guid.Empty).WithMessage("شناسه تراکنش معتبر نمی باشد.");
    }
}