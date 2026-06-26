using FluentValidation;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Commands.UpsertCourseSections
{
    public sealed class EpisodeDtoValidator : AbstractValidator<EpisodeDto>
    {
        private const long MaxVideoFileSizeInBytes = 2L * 1024 * 1024 * 1024; // 2GB
        private static readonly string[] AllowedVideoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm" };

        public EpisodeDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("عنوان اپیزود الزامی است")
                .MaximumLength(200)
                .WithMessage("عنوان اپیزود نباید بیشتر از 200 کاراکتر باشد");

            RuleFor(x => x.DisplayOrder)
                .GreaterThan(0)
                .WithMessage("ترتیب نمایش باید بزرگتر از صفر باشد");

            RuleFor(x => x.Duration)
                .Must(duration => duration > TimeSpan.Zero)
                .WithMessage("مدت زمان اپیزود باید بزرگتر از صفر باشد")
                .Must(duration => duration <= TimeSpan.FromHours(10))
                .WithMessage("مدت زمان اپیزود نباید بیشتر از 10 ساعت باشد");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("قیمت نمی‌تواند منفی باشد")
                .LessThanOrEqualTo(100_000_000)
                .WithMessage("قیمت نمی‌تواند بیشتر از 100 میلیون تومان باشد");

            // اگر اپیزود رایگان نیست، باید قیمت داشته باشد
            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("اپیزود غیررایگان باید قیمت داشته باشد")
                .When(x => !x.IsFree);

            // اگر فایل ویدئو آپلود شده، باید معتبر باشد
            When(x => x.VideoFile != null, () =>
            {
                RuleFor(x => x.VideoFile.Length)
                    .LessThanOrEqualTo(MaxVideoFileSizeInBytes)
                    .WithMessage($"حجم فایل ویدئو نباید بیشتر از {MaxVideoFileSizeInBytes / (1024 * 1024 * 1024)} گیگابایت باشد");

                RuleFor(x => x.VideoFile.FileName)
                    .Must(fileName => AllowedVideoExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    .WithMessage($"فرمت فایل ویدئو باید یکی از موارد زیر باشد: {string.Join(", ", AllowedVideoExtensions)}");
            });

            // اگر Id خالی نیست (ویرایش)، VideoFile اختیاری است
            // اگر Id خالی است (ایجاد جدید)، VideoFile یا VideoFileName باید وجود داشته باشد
            When(x => x.Id == Guid.Empty, () =>
            {
                RuleFor(x => x)
                    .Must(episode => episode.VideoFile != null || !string.IsNullOrWhiteSpace(episode.VideoFileName))
                    .WithMessage("برای اپیزود جدید، فایل ویدئو الزامی است");
            });
        }
    }
}