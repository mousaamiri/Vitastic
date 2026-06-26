using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Categories.Commands.UpdateDescription;

public sealed class UpdateCategoryDescriptionCommandValidator :AbstractValidator<UpdateCategoryDescriptionCommand>
{
    public UpdateCategoryDescriptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("شناسه دسته نمیتواند خالی باشد.")
            .NotEmpty().WithMessage("شناسه دسته نمیتواند خالی باشد.");
        RuleFor(x => x.Description)
            .MaximumLength(Description.MaxLength).WithMessage($"توضیحات دسته بندی نباید بیش از {Description.MaxLength} کاراکتر باشد.");
    }
}
