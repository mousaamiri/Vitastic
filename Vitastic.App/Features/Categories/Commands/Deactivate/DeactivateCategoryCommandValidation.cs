using FluentValidation;

namespace Vitastic.App.Features.Categories.Commands.Deactivate;

public sealed class DeactivateCategoryCommandValidation : AbstractValidator<DeactivateCategoryCommand>
{
    public DeactivateCategoryCommandValidation()
    {
        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty).WithMessage("شناسه برچسب نمیتواند مقدار خالی باشد")
            .NotEmpty().WithMessage("شناسه دسته‌بندی الزامی است");
    }
}
