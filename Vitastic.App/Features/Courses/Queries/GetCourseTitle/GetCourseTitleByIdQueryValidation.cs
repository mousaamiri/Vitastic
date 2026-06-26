using FluentValidation;
using Vitastic.App.Features.Courses.Queries.GetById;

namespace Vitastic.App.Features.Courses.Queries.GetCourseTitle
{
    public sealed class GetCourseTitleByIdQueryValidation : AbstractValidator<GetCourseByIdQuery>
    {
        public GetCourseTitleByIdQueryValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");
        }
    }
}