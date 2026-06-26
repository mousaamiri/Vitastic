using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.SetTaxAmount
{
    public sealed class SetTaxAmountCommandValidation : AbstractValidator<SetTaxAmountCommand>
    {
        public SetTaxAmountCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
            RuleFor(x => x.TaxAmount).GreaterThanOrEqualTo(0).WithMessage("مقدار مالیات نمی تواند منفی باشد.");
        }
    }
}