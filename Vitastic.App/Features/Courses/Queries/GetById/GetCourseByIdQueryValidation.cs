using FluentValidation;

namespace Vitastic.App.Features.Courses.Queries.GetById
{
    public sealed class GetCourseByIdQueryValidation : AbstractValidator<GetCourseByIdQuery>
    {
        public GetCourseByIdQueryValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");
            RuleFor(x => x)
                .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
                .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");
        }
    }
}
