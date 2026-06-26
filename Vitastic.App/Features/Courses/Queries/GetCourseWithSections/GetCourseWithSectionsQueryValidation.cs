using FluentValidation;

namespace Vitastic.App.Features.Courses.Queries.GetCourseWithSections
{
    public sealed class GetCourseWithSectionsQueryValidation : AbstractValidator<GetCourseWithSectionsQuery>
    {
        public GetCourseWithSectionsQueryValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x)
                .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
                .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");
        }
    }
}
