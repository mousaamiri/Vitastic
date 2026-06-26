using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.SetDemo
{
    public sealed class SetCourseDemoVideoCommandValidation : AbstractValidator<SetCourseDemoVideoCommand>
    {
        public SetCourseDemoVideoCommandValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            // RuleFor(x => x.VideoFile)
            //     .NotEmpty().WithMessage("نام ویدیو نمی تواند خالی باشد.")
            //     .MaximumLength(200).WithMessage("نام ویدیو نمی تواند بیشتر از 200 کاراکتر باشد.")
            //     .Matches(@"^[a-zA-Z0-9_\-\.\/]+\.(mp4|avi|mov|wmv|flv|mkv|webm)$").WithMessage("فرمت ویدیو معتبر نیست.");
        }
    }
}
