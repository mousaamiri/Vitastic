using FluentValidation;

namespace Vitastic.App.Features.Payments.Commands.AssignPaymentToOrder
{
    public sealed class AssignPaymentToOrderCommandValidator : AbstractValidator<AssignPaymentToOrderCommand>
    {
        public AssignPaymentToOrderCommandValidator()
        {
            RuleFor(x => x.TransactionId).NotEqual(Guid.Empty).WithMessage("شناسه تراکنش معتبر نمی باشد.");
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
        }
    }
}