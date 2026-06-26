using FluentValidation;

namespace Vitastic.App.Features.Users.Commands.CreateByAdmin;

public sealed class CreateUserByAdminCommandValidator : AbstractValidator<CreateUserByAdminCommand>
{
    #region Validation Rules

    public CreateUserByAdminCommandValidator()
    {
        // Validate UserName
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("نام کاربری نمی‌تواند خالی باشد.")
            .MinimumLength(3).WithMessage("نام کاربری باید حداقل ۳ کاراکتر داشته باشد.")
            .MaximumLength(50).WithMessage("نام کاربری نمی‌تواند بیش از ۵۰ کاراکتر باشد.");

        // Validate Email
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("ایمیل نمی‌تواند خالی باشد.")
            .EmailAddress().WithMessage("فرمت ایمیل معتبر نیست.");

        // Validate Password
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("رمز عبور نمی‌تواند خالی باشد.")
            .MinimumLength(6).WithMessage("رمز عبور باید حداقل ۶ کاراکتر داشته باشد.");

        // Validate FirstName
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("نام نمی‌تواند خالی باشد.")
            .MaximumLength(100).WithMessage("نام نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

        // Validate LastName
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("نام خانوادگی نمی‌تواند خالی باشد.")
            .MaximumLength(100).WithMessage("نام خانوادگی نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

        // Validate PhoneNumber (optional but must be valid if provided)
        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(\+98|0)?9\d{9}$")
                .WithMessage("فرمت شماره تلفن نامعتبر است (مثال: 09123456789 یا +989123456789)")
                .Must(phone =>
                {
                    // Normalize phone number
                    var normalized = phone.Replace("+98", "0").Replace(" ", "").Replace("-", "");
                    return normalized.Length == 11 && normalized.StartsWith("09");
                })
                .WithMessage("شماره تلفن باید ۱۱ رقم باشد و با ۰۹ شروع شود");
        });

        // Validate AvatarFile (optional)
        When(x => x.AvatarFile != null, () =>
        {
            // Ensure FileName is not empty
            RuleFor(x => x.AvatarFile!.FileName)
                .NotEmpty().WithMessage("نام فایل آواتار نمی‌تواند خالی باشد.")
                .MaximumLength(255).WithMessage("نام فایل آواتار نمی‌تواند بیش از ۲۵۵ کاراکتر باشد.");

            // Validate file extension
            RuleFor(x => x.AvatarFile)
                .Must(file => new[] { ".jpg", ".jpeg", ".png", ".gif" }
                    .Contains(Path.GetExtension(file.FileName ?? "").ToLowerInvariant()))
                .WithMessage("آواتار باید یک فایل تصویری با پسوند JPG، JPEG، PNG یا GIF باشد.");

            // Validate file size (max 5 MB)
            RuleFor(x => x.AvatarFile)
                .Must(file => file.Length <= 5 * 1024 * 1024)
                .WithMessage("حجم آواتار نباید بیش از ۵ مگابایت باشد.");
        });

        // Validate RoleIds
        RuleFor(x => x.RoleIds)
            .NotNull().WithMessage("لیست نقش‌ها نمی‌تواند null باشد.")
            .Must(list => list.Count > 0).WithMessage("حداقل یک نقش باید انتخاب شود.")
            .Must(list => list.All(id => id != Guid.Empty)).WithMessage("شناسه‌های نقش نباید خالی باشند.");
    }

    #endregion
}
