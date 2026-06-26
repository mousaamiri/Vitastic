using FluentValidation;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.AddSectionEpisode
{
    public sealed class AddCourseEpisodeCommandValidation : AbstractValidator<AddCourseEpisodeCommand>
    {
        public AddCourseEpisodeCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("شناسه بخش نمی تواند خالی باشد.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان قسمت نمی تواند خالی باشد.")
                .MaximumLength(EpisodeTitle.MaxLength).WithMessage($"عنوان قسمت نمی تواند بیشتر از {EpisodeTitle.MaxLength} کاراکتر باشد.")
                .MinimumLength(EpisodeTitle.MinLength).WithMessage($"عنوان قسمت نمی تواند کمتر از {EpisodeTitle.MinLength} کاراکتر باشد.");

            RuleFor(x => x.Duration)
                .GreaterThan(TimeSpan.Zero).WithMessage("مدت زمان باید بیشتر از صفر باشد.")
                .LessThanOrEqualTo(TimeSpan.FromHours(10)).WithMessage("مدت زمان نمی تواند بیشتر از 10 ساعت باشد.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("قیمت نمی تواند منفی باشد.");

            RuleFor(x => x.Currency)
                .MaximumLength(Currency.CodeLength).WithMessage($"واحد پول باید حداکثر {Currency.CodeLength} کاراکتر باشد.")
                .Matches(Currency.CodePattern).When(x => !string.IsNullOrEmpty(x.Currency))
                .WithMessage("واحد پول باید یک کد ارز معتبر 3 حرفی باشد (مثلاً IRR، USD).");

            When(x => x.VideoFile != null, () =>
            {
                // RuleFor(x => x.VideoFile!.FileName)
                //     .MaximumLength(CourseVideoName.MaxLength)
                //     .WithMessage($"نام فایل ویدیو نمی تواند بیشتر از {CourseVideoName.MaxLength} کاراکتر باشد.")
                //     .Matches(@"^[a-zA-Z0-9_\-\.\/]+\.(mp4|avi|mov|wmv|flv|mkv|webm)$")
                //     .When(x => !string.IsNullOrEmpty(x.VideoFileName))
                //     .WithMessage("فرمت فایل ویدیو معتبر نیست.");
            });
        }
    }
}
