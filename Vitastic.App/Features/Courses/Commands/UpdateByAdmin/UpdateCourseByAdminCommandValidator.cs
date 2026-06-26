using FluentValidation;
using FluentValidation.Validators;
using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Features.Courses.Commands.CreateByAdmin;

namespace Vitastic.App.Features.Courses.Commands.UpdateByAdmin;

public sealed class UpdateCourseByAdminCommandValidator : AbstractValidator<UpdateCourseByAdminCommand>
{
    private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB
    private const long MaxVideoSize = 20 * 1024 * 1024; // 20 MB

    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    private static readonly string[] AllowedVideoTypes = { "video/mp4", "video/webm", "video/quicktime" };

    public UpdateCourseByAdminCommandValidator()
    {

        // CourseId
        RuleFor(x => x.CourseId)
            .NotEqual(Guid.Empty).WithMessage("شناسه دوره نامعتبر است.");

        // Title
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان دوره نمی‌تواند خالی باشد.")
            .MinimumLength(5).WithMessage("عنوان دوره باید حداقل ۵ کاراکتر داشته باشد.")
            .MaximumLength(200).WithMessage("عنوان دوره نمی‌تواند بیش از ۲۰۰ کاراکتر باشد.");

        // Description
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("توضیحات دوره نمی‌تواند خالی باشد.")
            .MinimumLength(20).WithMessage("توضیحات دوره باید حداقل ۲۰ کاراکتر داشته باشد.");

        // ShortDescription
        RuleFor(x => x.ShortDescription)
            .NotEmpty().WithMessage("توضیحات کوتاه نمی‌تواند خالی باشد.")
            .MaximumLength(300).WithMessage("توضیحات کوتاه نباید بیش از ۳۰۰ کاراکتر باشد.");

        // Slug
        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("آدرس دوره (Slug) نمی‌تواند خالی باشد.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("آدرس دوره باید شامل حروف کوچک، اعداد و خط‌تیره باشد.");

        // Level
        RuleFor(x => x.Level)
            .IsInEnum().WithMessage("سطح دوره نامعتبر است.");

        // InstructorId
        RuleFor(x => x.InstructorId)
            .NotEqual(Guid.Empty).WithMessage("شناسه مدرس نامعتبر است.");

        // Tags
        When(x => x.TagIds != null, () =>
        {
            RuleFor(x => x.TagIds)
                .NotNull().WithMessage("لیست برچسب‌ها نمی‌تواند null باشد.")
                .Must(list => list!.Count > 0).WithMessage("حداقل یک برچسب باید انتخاب شود.")
                .Must(list => list.All(id => id != Guid.Empty)).WithMessage("شناسه برچسب نباید خالی باشد.");
        });

        // Categories
        When(x => x.TagIds != null, () =>
        {
            RuleFor(x => x.CategoryIds)
                .NotNull().WithMessage("لیست دسته‌بندی‌ها نمی‌تواند null باشد.")
                .Must(list => list!.Count > 0).WithMessage("حداقل یک دسته‌بندی باید انتخاب شود.")
                .Must(list => list.All(id => id != Guid.Empty)).WithMessage("شناسه دسته‌بندی نباید خالی باشد.");
        });

        // Image (optional)
        RuleFor(x => x.ImageFile)
            .SetValidator(new FileValidator(
                maxSize: MaxImageSize,
                allowedContentTypes: AllowedImageTypes,
                fileTypeDisplayName: "تصویر"))
            .When(x => x.ImageFile is not null);

        RuleFor(x => x.ThumbnailFile)
            .SetValidator(new FileValidator(
                maxSize: MaxImageSize,
                allowedContentTypes: AllowedImageTypes,
                fileTypeDisplayName: "تصویر بندانگشتی"));

        // Demo Video (optional)
        RuleFor(x => x.DemoVideoFile)
            .SetValidator(new FileValidator(
                maxSize: MaxVideoSize,
                allowedContentTypes: AllowedVideoTypes,
                fileTypeDisplayName: "ویدیوی نمایشی"))
            .When(x => x.DemoVideoFile is not null);
    }

    private sealed class FileValidator(
        long maxSize,
        string[] allowedContentTypes,
        string fileTypeDisplayName)
        : PropertyValidator<UpdateCourseByAdminCommand, IFile?>
    {
        public override string Name => nameof(FileValidator);

        public override bool IsValid(ValidationContext<UpdateCourseByAdminCommand> context, IFile? value)
        {
            if (value is null)
                return true;

            // File name
            if (string.IsNullOrWhiteSpace(value.FileName))
            {
                context.MessageFormatter.AppendArgument("Message",
                    $"نام فایل {fileTypeDisplayName} نامعتبر است.");

                return false;
            }

            if (value.FileName.Length > 255)
            {
                context.MessageFormatter.AppendArgument("Message",
                    $"نام فایل {fileTypeDisplayName} نباید بیشتر از 255 کاراکتر باشد.");

                return false;
            }

            // File size
            if (value.Length <= 0)
            {
                context.MessageFormatter.AppendArgument("Message",
                    $"فایل {fileTypeDisplayName} خالی است.");

                return false;
            }

            if (value.Length > maxSize)
            {
                var maxSizeMb = maxSize / 1024 / 1024;

                context.MessageFormatter.AppendArgument("Message",
                    $"حجم فایل {fileTypeDisplayName} نباید بیشتر از {maxSizeMb} مگابایت باشد.");

                return false;
            }

            // Content type
            if (string.IsNullOrWhiteSpace(value.ContentType) ||
                !allowedContentTypes.Contains(
                    value.ContentType,
                    StringComparer.OrdinalIgnoreCase))
            {
                context.MessageFormatter.AppendArgument("Message",
                    $"فرمت فایل {fileTypeDisplayName} معتبر نیست.");

                return false;
            }

            return true;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "{Message}";
        }
    }
}
