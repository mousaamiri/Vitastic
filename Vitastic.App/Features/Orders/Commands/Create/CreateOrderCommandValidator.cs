using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.Create
{
    public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("شناسه کاربری معتبر نیست.");
        }
    }
}
