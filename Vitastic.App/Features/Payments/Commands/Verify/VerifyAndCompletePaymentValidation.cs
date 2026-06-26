using FluentValidation;

namespace Vitastic.App.Features.Payments.Commands.Verify
{
    public sealed class VerifyAndCompletePaymentValidation : AbstractValidator<VerifyAndCompletePaymentCommand>
    {
        public VerifyAndCompletePaymentValidation()
        {

            RuleFor(x => x.Authority).NotEmpty().WithMessage("اوتوریتی پرداخت نمی‌تواند خالی باشد.")
                .MaximumLength(100).WithMessage("اوتوریتی پرداخت نمی‌تواند بیشتر از 100 کاراکتر باشد.");
            RuleFor(x => x.Status).NotEmpty().WithMessage("وضعیت پرداخت نمی‌تواند خالی باشد.")
                .MaximumLength(50).WithMessage("وضعیت پرداخت نمی‌تواند بیشتر از 50 کاراکتر باشد.");
            RuleFor(x => x.CallbackRefId).MaximumLength(100).WithMessage("شناسه مرجع بازگشتی نمی‌تواند بیشتر از 100 کاراکتر باشد.");

        }
    }
}