using FluentValidation;

namespace Vitastic.App.Features.Payments.Commands.Cancel
{
    public  sealed class CancelPaymentCommandValidation: AbstractValidator<CancelPaymentCommand>
    {
        public CancelPaymentCommandValidation()
        {
            RuleFor(x => x.TransactionId).NotEqual(Guid.Empty).WithMessage("شناسه تراکنش نمی تواند خالی باشد.");
        }
    }
}