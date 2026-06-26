using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Queries.GetBySlug
{
    public sealed class GetCourseBySlugQueryValidation : AbstractValidator<GetCourseBySlugQuery>
    {
        public GetCourseBySlugQueryValidation()
        {
            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("نامک نمی تواند خالی باشد.")
                .MaximumLength(Slug.MaxLength).WithMessage($"نامک نمی تواند بیشتر از {Slug.MaxLength} کاراکتر باشد.")
                .MinimumLength(Slug.MinLength).WithMessage($"نامک نمی تواند کمتر از {Slug.MinLength} کاراکتر باشد.")
                .Matches(Slug.Pattern).WithMessage("نامک باید فقط شامل حروف کوچک، اعداد و علائم نگارشی مجاز باشد.");

            RuleFor(x => x)
                .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
                .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");
        }
    }
}
