using FluentValidation;
using Vitastic.Domain.Entities.Instructors.ValueObjects;

namespace Vitastic.App.Features.Instructors.Commands.Update;


public sealed class UpdateInstructorCommandValidator : AbstractValidator<UpdateInstructorCommand>
{
    private static readonly string[] AllowedImageContentTypes =
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp"
    };

    public UpdateInstructorCommandValidator()
    {
        RuleFor(c => c.InstructorId)
            .NotEqual(Guid.Empty)
            .WithMessage("شناسه استاد نمی‌تواند خالی باشد.");

        // Biography is optional → only when checked
        RuleFor(c => c.NewBio)
            .MaximumLength(InstructorBio.MaxLength)
            .WithMessage($"بیوگرافی نمی‌تواند بیشتر از {InstructorBio.MaxLength} کاراکتر باشد.")
            .MinimumLength(InstructorBio.MinLength)
            .When(c => !string.IsNullOrWhiteSpace(c.NewBio))
            .WithMessage($"بیوگرافی باید حداقل {InstructorBio.MinLength} کاراکتر باشد (در صورت وارد شدن).");
    }

    private static bool BeAValidImageContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType)) return false;
        return AllowedImageContentTypes.Contains(contentType.Trim().ToLowerInvariant());
    }

    private static bool HaveValidImageExtension(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return false;

        var extension = Path.GetExtension(fileName)
                              .TrimStart('.')
                              .ToLowerInvariant();

        return extension is "jpg" or "jpeg" or "png" or "webp";
    }
}
