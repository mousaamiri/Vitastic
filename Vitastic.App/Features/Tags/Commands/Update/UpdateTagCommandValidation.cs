using FluentValidation;
using Vitastic.App.Features.Tags.Commands.Create;
using Vitastic.Domain.Entities.Tags.ValueObjects;

namespace Vitastic.App.Features.Tags.Commands.Update;

public sealed class UpdateTagCommandValidation:AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("شناسه برچسب نباید پیشفرض خالی باشد.")
            .NotEqual(Guid.Empty).WithMessage("شناسه برچسب نباید پیشفرض خالی باشد.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام دسته‌بندی الزامی است")
            .MinimumLength(TagName.MinLength).WithMessage($"نام باید حداقل {TagName.MinLength} کاراکتر باشد")
            .MaximumLength(TagName.MaxLength).WithMessage($"نام نمی‌تواند بیش از {TagName.MaxLength} کاراکتر باشد");

    }
}
