using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.AddCustomerNote
{
    public sealed class AddCustomerNoteCommandValidation : AbstractValidator<AddCustomerNoteCommand>
    {
        public AddCustomerNoteCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
            RuleFor(x => x.Note).NotEmpty().WithMessage("یادداشت مشتری نمی تواند خالی باشد.")
                .MaximumLength(1000).WithMessage("یادداشت مشتری باید حداکثر 1000 کاراکتر باشد.");
        }
    }
}