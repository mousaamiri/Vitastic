using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.SetTagList
{
    public sealed class SetCourseTagsCommandValidation : AbstractValidator<SetCourseTagsCommand>
    {
        public SetCourseTagsCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.TagIds)
                .NotNull().WithMessage("لیست برچسب‌ها نمی تواند خالی باشد.")
                .Must(x => x.Count == x.Distinct().Count()).WithMessage("لیست برچسب‌ها نمی تواند دارای مقادیر تکراری باشد.");
        }
    }
}