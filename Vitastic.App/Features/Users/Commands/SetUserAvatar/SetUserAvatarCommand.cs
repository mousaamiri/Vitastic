using FluentValidation;
using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Commands.SetUserAvatar;

public record SetUserAvatarCommand(Guid UserId,IFile AvatarFile) : ICommand;

public class SetUserAvatarCommandValidator : AbstractValidator<SetUserAvatarCommand>
{
    #region Constants
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly string[] AllowedMimeTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };
    #endregion

    public SetUserAvatarCommandValidator()
    {
        #region UserId Validation
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("شناسه کاربر الزامی است.");
        #endregion

        #region AvatarFile Validation
        RuleFor(x => x.AvatarFile)
            .NotNull()
            .WithMessage("فایل تصویر الزامی است.");

        // File size validation
        RuleFor(x => x.AvatarFile.Length)
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .When(x => x.AvatarFile != null)
            .WithMessage($"حجم فایل نباید بیشتر از {MaxFileSizeBytes / 1024 / 1024} مگابایت باشد.");

        // File extension validation
        RuleFor(x => x.AvatarFile.FileName)
            .Must(fileName =>
            {
                if (string.IsNullOrEmpty(fileName)) return false;
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                return AllowedExtensions.Contains(extension);
            })
            .When(x => x.AvatarFile != null)
            .WithMessage($"فرمت فایل باید یکی از موارد زیر باشد: {string.Join(", ", AllowedExtensions)}");

        // MIME type validation
        RuleFor(x => x.AvatarFile.ContentType)
            .Must(contentType => AllowedMimeTypes.Contains(contentType?.ToLowerInvariant()))
            .When(x => x.AvatarFile != null)
            .WithMessage("نوع فایل معتبر نیست.");
        #endregion
        // Validate actual file content (not just extension)
        RuleFor(x => x.AvatarFile)
            .MustAsync(async (file, ct) => await IsValidImageAsync(file, ct))
            .When(x => x.AvatarFile != null)
            .WithMessage("محتوای فایل معتبر نیست.");


    }
    private static async Task<bool> IsValidImageAsync(IFile file, CancellationToken ct)
             {
                 try
                 {
                     // Read first few bytes to check magic numbers
                     using var stream = file.OpenReadStream();
                     var buffer = new byte[8];
                     await stream.ReadAsync(buffer, 0, 8, ct);

                     // Check magic numbers for common image formats
                     // JPEG: FF D8 FF
                     if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                         return true;

                     // PNG: 89 50 4E 47
                     if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                         return true;

                     // WebP: 52 49 46 46 ... 57 45 42 50
                     if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46)
                         return true;

                     return false;
                 }
                 catch
                 {
                     return false;
                 }
             }

}

public class SetUserAvatarCommandHandler
(IUserRepository userRepository,
    IFileStorageService fileStorageService)
    : ICommandHandler<SetUserAvatarCommand>
{
    public async Task<Result> Handle(SetUserAvatarCommand request, CancellationToken cancellationToken)
    {
        // 1. Convert user id to value object
        var userIdResult = UserId.CreateFrom(request.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;

        // 2. Get user from database
        User? user = await userRepository.FindAsync(userIdResult.Value, cancellationToken);
        if (user is null)
            return Error.NotFound("SetUserAvatarCommand.UserNotFound", "کاربر یافت نشد.");

        // 3. Save avatar file to storage
        var fileName = await fileStorageService.UploadFileAsync(
            request.AvatarFile,
            nameof(User),
            userIdResult.Value.Value,
            FileType.Image);

        // 4. Update user avatar
        var userAvatarResult = UserAvatar.Create(fileName);
        if (userAvatarResult.IsFailure)
            return userAvatarResult.Error;

        user.ChangeAvatar(userAvatarResult.Value);
        await userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}

