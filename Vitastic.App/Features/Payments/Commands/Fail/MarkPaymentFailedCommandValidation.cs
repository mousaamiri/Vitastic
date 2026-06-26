using FluentValidation;

namespace Vitastic.App.Features.Payments.Commands.Fail
{
    public sealed class MarkPaymentFailedCommandValidation : AbstractValidator<MarkPaymentFailedCommand>
    {
        public MarkPaymentFailedCommandValidation()
        {
            RuleFor(x => x.TransactionId).NotEqual(Guid.Empty).WithMessage("شناسه تراکنش معتبر نمی باشد.");
        }
    }
}