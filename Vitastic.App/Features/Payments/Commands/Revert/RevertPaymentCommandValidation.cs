using FluentValidation;

namespace Vitastic.App.Features.Payments.Commands.Revert
{
    public sealed class RevertPaymentCommandValidation : AbstractValidator<RevertPaymentCommand>
    {
        public RevertPaymentCommandValidation()
        {
            RuleFor(x => x.TransactionId).NotEqual(Guid.Empty).WithMessage("شناسه تراکنش معتبر نمی باشد.");
        }
    }
}