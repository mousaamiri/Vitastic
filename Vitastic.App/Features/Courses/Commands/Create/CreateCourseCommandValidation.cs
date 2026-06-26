using FluentValidation;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.Create
{
    public sealed class CreateCourseCommandValidation : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidation()
        {
            // Validation for InstructorId
            RuleFor(x => x.InstructorId)
                .NotEqual(Guid.Empty).WithMessage("شناسه مدرس نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه مدرس نمی تواند خالی باشد.");

            // Validation for Title
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان دوره نمی تواند خالی باشد.")
                .MaximumLength(CourseTitle.MaxLength).WithMessage($"عنوان دوره نمی تواند بیشتر از {CourseTitle.MaxLength} کاراکتر باشد.")
                .MinimumLength(CourseTitle.MinLength).WithMessage($"عنوان دوره نمی تواند کمتر از {CourseTitle.MinLength} کاراکتر باشد.");

            // Validation for Description
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("توضیحات نمی تواند خالی باشد.")
                .MaximumLength(Description.MaxLength).WithMessage($"توضیحات نمی تواند بیشتر از {Description.MaxLength} کاراکتر باشد.")
                .MinimumLength(Description.MinLength).WithMessage($"توضیحات نمی تواند کمتر از {Description.MinLength} کاراکتر باشد.");

            // Validation for ShortDescription
            RuleFor(x => x.ShortDescription)
                .NotEmpty().WithMessage("توضیحات کوتاه نمی تواند خالی باشد.")
                .MinimumLength(ShortDescription.MinLength).WithMessage($"توضیحات کوتاه باید حداقل {ShortDescription.MinLength} کاراکتر باشد.")
                .MaximumLength(ShortDescription.MaxLength).WithMessage($"توضیحات کوتاه نمی تواند بیشتر از {ShortDescription.MaxLength} کاراکتر باشد.");

            // Validation for Slug
            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("نامک نمی تواند خالی باشد.")
                .MaximumLength(Slug.MaxLength).WithMessage($"نامک نمی تواند بیشتر از {Slug.MaxLength} کاراکتر باشد.")
                .MinimumLength(Slug.MinLength).WithMessage($"نامک نمی تواند کمتر از {Slug.MinLength} کاراکتر باشد.")
                .Matches(Slug.Pattern).WithMessage("نامک باید فقط شامل حروف کوچک، اعداد و علائم نگارشی مجاز باشد.");

            // Validation for Level
            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("سطح دوره معتبر نیست.");

            // Validation for Currency (optional)
            RuleFor(x => x.Currency)
                .MaximumLength(Currency.CodeLength).WithMessage($"واحد پول باید حداکثر {Currency.CodeLength} کاراکتر باشد.")
                .Matches(Currency.CodePattern).When(x => !string.IsNullOrEmpty(x.Currency))
                .WithMessage("واحد پول باید یک کد ارز معتبر 3 حرفی باشد (مثلاً IRR، USD).");
        }
    }
}