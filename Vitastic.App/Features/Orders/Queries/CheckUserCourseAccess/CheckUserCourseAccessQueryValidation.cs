using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.CheckUserCourseAccess
{
    public sealed class CheckUserCourseAccessQueryValidation : AbstractValidator<CheckUserCourseAccessQuery>
    {
        public CheckUserCourseAccessQueryValidation()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("شناسه کاربر معتبر نمی باشد.");
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره معتبر نمی باشد.");
        }
    }
}