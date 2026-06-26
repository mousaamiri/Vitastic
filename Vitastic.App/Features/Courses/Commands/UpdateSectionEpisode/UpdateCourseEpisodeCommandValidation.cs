using FluentValidation;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.UpdateSectionEpisode
{
    public sealed class UpdateCourseEpisodeCommandValidation : AbstractValidator<UpdateCourseEpisodeCommand>
    {
        public UpdateCourseEpisodeCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("شناسه بخش نمی تواند خالی باشد.");

            RuleFor(x => x.EpisodeId)
                .NotEmpty().WithMessage("شناسه قسمت نمی تواند خالی باشد.");

            When(x => x.Title != null, () =>
            {
                RuleFor(x => x.Title!)
                    .NotEmpty().WithMessage("عنوان قسمت نمی تواند خالی باشد.")
                    .MaximumLength(EpisodeTitle.MaxLength).WithMessage($"عنوان قسمت نمی تواند بیشتر از {EpisodeTitle.MaxLength} کاراکتر باشد.")
                    .MinimumLength(EpisodeTitle.MinLength).WithMessage($"عنوان قسمت نمی تواند کمتر از {EpisodeTitle.MinLength} کاراکتر باشد.");
            });

            When(x => x.Duration.HasValue, () =>
            {
                RuleFor(x => x.Duration!.Value)
                    .GreaterThan(TimeSpan.Zero).WithMessage("مدت زمان باید بیشتر از صفر باشد.")
                    .LessThanOrEqualTo(TimeSpan.FromHours(10)).WithMessage("مدت زمان نمی تواند بیشتر از 10 ساعت باشد.");
            });

            When(x => x.Price.HasValue, () =>
            {
                RuleFor(x => x.Price!.Value)
                    .GreaterThanOrEqualTo(0).WithMessage("قیمت نمی تواند منفی باشد.");
            });

            When(x => x.Currency != null, () =>
            {
                RuleFor(x => x.Currency!)
                    .MaximumLength(Currency.CodeLength).WithMessage($"واحد پول باید حداکثر {Currency.CodeLength} کاراکتر باشد.")
                    .Matches(Currency.CodePattern).WithMessage("واحد پول باید یک کد ارز معتبر 3 حرفی باشد (مثلاً IRR، USD).");
            });

            // When(x => x.VideoFile != null, () =>
            // {
            //     RuleFor(x => x.VideoFile!)
            //         .MaximumLength(200).WithMessage("نام فایل ویدیو نمی تواند بیشتر از 200 کاراکتر باشد.")
            //         .Matches(@"^[a-zA-Z0-9_\-\.\/]+\.(mp4|avi|mov|wmv|flv|mkv|webm)$").WithMessage("فرمت فایل ویدیو معتبر نیست.");
            // });
        }
    }
}
