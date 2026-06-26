using FluentValidation;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Categories.Commands.Update;

public sealed class UpdateCategoryValidator :AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("شناسه دسته نمیتواند خالی باشد.")
            .NotEmpty().WithMessage("شناسه دسته نمیتواند خالی باشد.");
        RuleFor(x => x.Name)
            .MaximumLength(TagName.MaxLength).WithMessage($"نام دسته نباید بیش از {TagName.MaxLength} کاراکتر باشد. ");

        RuleFor(x => x.Description)
            .MaximumLength(Description.MaxLength).WithMessage($"توضیحات دسته بندی نباید بیش از {Description.MaxLength} کاراکتر باشد.");
    }
}
