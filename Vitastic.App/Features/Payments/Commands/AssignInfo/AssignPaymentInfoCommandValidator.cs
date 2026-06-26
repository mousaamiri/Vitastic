using FluentValidation;

namespace Vitastic.App.Features.Payments.Commands.AssignInfo
{
    public sealed class AssignPaymentInfoCommandValidator : AbstractValidator<AssignPaymentInfoCommand>
    {
        public AssignPaymentInfoCommandValidator()
        {
            RuleFor(x => x.TransactionId).NotEqual(Guid.Empty).WithMessage("شناسه تراکنش معتبر نمی باشد.");
            RuleFor(x => x.Authority).NotEmpty().WithMessage("شماره مرجع پرداخت الزامی است.");
            RuleFor(x => x.Provider).NotEmpty().WithMessage("نام ارائه دهنده پرداخت الزامی است.");
        }
    }
}