using FluentValidation;

namespace Vitastic.App.Features.Categories.Commands.UpdateDisplayOrder;

public sealed class UpdateCategoryDisplayOrderCommandValidator :AbstractValidator<UpdateCategoryDisplayOrderCommand>
{
    public UpdateCategoryDisplayOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("شناسه دسته نمیتواند خالی باشد.")
            .NotEmpty().WithMessage("شناسه دسته نمیتواند خالی باشد.");
        RuleFor(x => x.DisplayOrder)
            .GreaterThan(0).WithMessage("ترتیب نمایش باید بیشتر از صفر باشد.");
    }
}
