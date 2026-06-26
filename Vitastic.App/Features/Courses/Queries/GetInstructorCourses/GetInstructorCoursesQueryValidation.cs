using FluentValidation;

namespace Vitastic.App.Features.Courses.Queries.GetInstructorCourses
{
    public sealed class GetInstructorCoursesQueryValidation : AbstractValidator<GetInstructorCoursesQuery>
    {
        public GetInstructorCoursesQueryValidation()
        {
            // Instructor validation
            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("شناسه مدرس نمی تواند خالی باشد.")
                .NotEqual(Guid.Empty).WithMessage("شناسه مدرس نمی تواند خالی باشد.")
                ;

            // Pagination validation
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("شماره صفحه باید بزرگتر یا مساوی 1 باشد.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("تعداد آیتم در صفحه باید بزرگتر یا مساوی 1 باشد.")
                .LessThanOrEqualTo(100).WithMessage("تعداد آیتم در صفحه نمی تواند بیشتر از 100 باشد.");
        }
    }
}