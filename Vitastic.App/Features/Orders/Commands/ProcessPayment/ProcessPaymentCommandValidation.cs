using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.ProcessPayment
{
    public sealed class ProcessPaymentCommandValidation : AbstractValidator<ProcessPaymentCommand>
    {
        public ProcessPaymentCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
            RuleFor(x => x.TransactionId).NotEqual(Guid.Empty).WithMessage("شناسه تراکنش معتبر نمی باشد.");
            RuleFor(x => x.PaymentMethodResponse)
                .IsInEnum().NotEmpty().WithMessage("شیوه پرداخت نمی تواند خالی باشد.");
        }
    }
}
