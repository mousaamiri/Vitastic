using FluentValidation;

namespace Vitastic.App.Features.Users.Commands.UpdateUserByAdmin
{
    public sealed class UpdateUserByAdminCommandValidator : AbstractValidator<UpdateUserByAdminCommand>
    {
        public UpdateUserByAdminCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("شناسه کاربر الزامی است.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("نام کاربری الزامی است")
                .MinimumLength(3).WithMessage("نام کاربری باید حداقل ۳ کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام کاربری نباید بیشتر از ۵۰ کاراکتر باشد")
                .Matches("^[a-zA-Z0-9_.-]+$")
                .WithMessage("نام کاربری فقط می‌تواند شامل حروف انگلیسی، اعداد و کاراکترهای _ . - باشد");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ایمیل الزامی است")
                .EmailAddress().WithMessage("فرمت ایمیل نامعتبر است")
                .MaximumLength(100).WithMessage("ایمیل نباید بیشتر از ۱۰۰ کاراکتر باشد");

            // Password is optional — validate only if provided
            When(x => !string.IsNullOrEmpty(x.Password), () =>
            {
                RuleFor(x => x.Password)
                    .MinimumLength(6).WithMessage("رمز عبور باید حداقل ۶ کاراکتر باشد")
                    .MaximumLength(100).WithMessage("رمز عبور نباید بیشتر از ۱۰۰ کاراکتر باشد");
            });

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("نام الزامی است")
                .MinimumLength(2).WithMessage("نام باید حداقل ۲ کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام نباید بیشتر از ۵۰ کاراکتر باشد");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("نام خانوادگی الزامی است")
                .MinimumLength(2).WithMessage("نام خانوادگی باید حداقل ۲ کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام خانوادگی نباید بیشتر از ۵۰ کاراکتر باشد");

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


            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("لیست نقش‌ها نمی‌تواند null باشد")
                .Must(roles => roles.Count > 0).WithMessage("حداقل یک نقش باید انتخاب شود")
                .Must(roles => roles.All(id => id != Guid.Empty)).WithMessage("شناسه نقش‌ها نامعتبر است");
        }
    }
}
