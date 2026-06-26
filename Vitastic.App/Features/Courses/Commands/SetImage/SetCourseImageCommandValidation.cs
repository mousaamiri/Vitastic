using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.SetImage
{
    public sealed class SetCourseImageCommandValidation : AbstractValidator<SetCourseImageCommand>
    {
        public SetCourseImageCommandValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            // RuleFor(x => x.ImageFile)
            //     .NotEmpty().WithMessage("نام تصویر نمی تواند خالی باشد.")
            //     .MaximumLength(200).WithMessage("نام تصویر نمی تواند بیشتر از 200 کاراکتر باشد.")
            //     .Matches(@"^[a-zA-Z0-9_\-\.\/]+\.(jpg|jpeg|png|gif|bmp|webp)$").WithMessage("فرمت تصویر معتبر نیست.");
            //
            // RuleFor(x => x.ThumbnailName)
            //     .MaximumLength(200).WithMessage("نام تصویر بند انگشتی نمی تواند بیشتر از 200 کاراکتر باشد.")
            //     .Matches(@"^[a-zA-Z0-9_\-\.\/]+\.(jpg|jpeg|png|gif|bmp|webp)$").When(x => !string.IsNullOrEmpty(x.ThumbnailName))
            //     .WithMessage("فرمت تصویر بند انگشتی معتبر نیست.");
        }
    }
}
