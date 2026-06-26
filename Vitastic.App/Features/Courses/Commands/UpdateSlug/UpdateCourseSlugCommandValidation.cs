using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.UpdateSlug
{
    public sealed class UpdateCourseSlugCommandValidation : AbstractValidator<UpdateCourseSlugCommand>
    {
        public UpdateCourseSlugCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("نامک نمی تواند خالی باشد.")
                .MaximumLength(Slug.MaxLength).WithMessage($"نامک نمی تواند بیشتر از {Slug.MaxLength} کاراکتر باشد.")
                .MinimumLength(Slug.MinLength).WithMessage($"نامک نمی تواند کمتر از {Slug.MinLength} کاراکتر باشد.")
                .Matches(Slug.Pattern).WithMessage("نامک باید فقط شامل حروف کوچک، اعداد و علائم نگارشی مجاز باشد.");
        }
    }
}