using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.AddAdminNote
{
    public sealed class AddAdminNoteCommandValidation : AbstractValidator<AddAdminNoteCommand>
    {
        public AddAdminNoteCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
            RuleFor(x => x.Note).NotEmpty().WithMessage("یادداشت مدیر نمی تواند خالی باشد.")
                .MaximumLength(1000).WithMessage("یادداشت مدیر باید حداکثر 1000 کاراکتر باشد.");
        }
    }
}