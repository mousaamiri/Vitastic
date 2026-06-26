using FluentValidation;

namespace Vitastic.App.Features.Discounts.Commands.AddCourseToDiscount
{
    public sealed class AddCourseToDiscountCommandValidation : AbstractValidator<AddCourseToDiscountCommand>
    {
        public AddCourseToDiscountCommandValidation()
        {
            RuleFor(x => x.DiscountId)
                .NotEqual(Guid.Empty).WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.");
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی‌تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی‌تواند خالی باشد.");
        }
    }
}