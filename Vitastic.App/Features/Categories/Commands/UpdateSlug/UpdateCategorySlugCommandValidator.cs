using FluentValidation;
using Vitastic.App.Features.Categories.Commands.UpdateName;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Categories.Commands.UpdateSlug;

public sealed class UpdateCategorySlugCommandValidator :AbstractValidator<UpdateCategorySlugCommand>
{
    public UpdateCategorySlugCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("شناسه دسته نمیتواند خالی باشد.")
            .NotEmpty().WithMessage("شناسه دسته نمیتواند خالی باشد.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("نامک نمی تواند خالی باشد.")
            .MaximumLength(Slug.MaxLength).WithMessage($"نامک نمی تواند بیشتر از {Slug.MaxLength} کاراکتر باشد.")
            .MinimumLength(Slug.MinLength).WithMessage($"نامک نمی تواند کمتر از {Slug.MinLength} کاراکتر باشد.")
            .Matches(Slug.Pattern).WithMessage("نامک باید فقط شامل حروف کوچک، اعداد و علائم نگارشی مجاز باشد.");
    }
}
