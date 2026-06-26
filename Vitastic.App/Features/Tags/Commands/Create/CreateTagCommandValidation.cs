using FluentValidation;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Tags.Commands.Create;

public sealed class CreateTagCommandValidation:AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام برچسب الزامی است")
            .MinimumLength(TagName.MinLength).WithMessage($"نام باید حداقل {TagName.MinLength} کاراکتر باشد")
            .MaximumLength(TagName.MaxLength).WithMessage($"نام نمی‌تواند بیش از {TagName.MaxLength} کاراکتر باشد");

    }
}
