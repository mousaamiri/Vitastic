using FluentValidation;
using Vitastic.Domain.Entities.Tags.ValueObjects;

namespace Vitastic.App.Features.Categories.Commands.UpdateName;

public sealed class UpdateCategoryNameCommandValidator :AbstractValidator<UpdateCategoryNameCommand>
{
    public UpdateCategoryNameCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("شناسه دسته نمیتواند خالی باشد.")
            .NotEmpty().WithMessage("شناسه دسته نمیتواند خالی باشد.");
        RuleFor(x => x.Name)
            .MaximumLength(TagName.MaxLength).WithMessage($"نام دسته نباید بیش از {TagName.MaxLength} کاراکتر باشد. ");
    }
}
