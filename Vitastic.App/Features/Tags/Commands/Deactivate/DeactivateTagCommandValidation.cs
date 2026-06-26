using FluentValidation;

namespace Vitastic.App.Features.Tags.Commands.Deactivate;

public sealed class DeactivateTagCommandValidation:AbstractValidator<DeactivateTagCommand>
{
    public DeactivateTagCommandValidation()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("شناسه برچسب نمیتواند مقدار خالی باشد باشد.")
            .NotEmpty().WithMessage("شناسه برچسب نمیتواند خالی باشد.");
    }
}
