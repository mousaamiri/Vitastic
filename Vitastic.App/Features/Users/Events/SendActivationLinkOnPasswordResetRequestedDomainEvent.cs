using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Users.Events;

namespace Vitastic.App.Features.Users.Events;

#region ==================== Send Activation Link on User Registered ====================

/// <summary>
/// Sends activation email when a new user registers
/// </summary>
public sealed class SendActivationLinkOnUserRegisteredDomainEventHandler(
    IEmailSender emailSender,
    ILogger<SendActivationLinkOnUserRegisteredDomainEventHandler> logger)
    : IEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(
        UserRegisteredDomainEvent notification,
        CancellationToken cancellationToken)
    {
        // Build activation link using the callback URL from the event
        var activationLink = $"{notification.CallbackBaseUrl}?token={notification.ActivateCode}";

        string body = $"""
                       <div dir="rtl" style="font-family: Tahoma, sans-serif;">
                           <h2>سلام {notification.UserName} عزیز</h2>
                           <p>برای فعال‌سازی حساب کاربری روی لینک زیر کلیک کنید:</p>
                           <a href="{activationLink}"
                              style="display:inline-block; padding:12px 24px;
                       background:#4CAF50; color:white;
                                     text-decoration:none; border-radius:6px;">فعال‌سازی حساب
                           </a>
                           <p>این لینک تا ۲۴ ساعت معتبر است.</p></div>
                       """;

        await emailSender.SendEmailAsync(
            notification.Email,
            "فعال‌سازی حساب کاربری",
            body,
            cancellationToken);

        logger.LogInformation(
            "Activation email sent — UserId: {UserId}, Email: {Email}",
            notification.UserId, notification.Email);
    }
}

#endregion

#region ==================== Send Reset Password Link ====================

/// <summary>
/// Sends password reset email when user requests password reset
/// </summary>
public sealed class SendResetPasswordLinkOnPasswordResetRequestedDomainEventHandler(
    IEmailSender emailSender,
    ILogger<SendResetPasswordLinkOnPasswordResetRequestedDomainEventHandler> logger)
    : IEventHandler<PasswordResetRequestedDomainEvent>
{
    public async Task Handle(
        PasswordResetRequestedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        // Build reset link using the callback URL from the event
        var resetLink = $"{notification.CallbackBaseUrl}?token={notification.ResetPasswordToken}";

        string body = $"""
                       <div dir="rtl" style="font-family: Tahoma, sans-serif;">
                           <h2>سلام {notification.UserName} عزیز</h2>
                           <p>برای بازیابی رمز عبور روی لینک زیر کلیک کنید:</p>
                           <a href="{resetLink}"
                              style="display:inline-block; padding:12px 24px;
                                     background:#2196F3; color:white;
                                     text-decoration:none; border-radius:6px;">
                              بازیابی رمز عبور
                           </a>
                           <p>این لینک تا ۲ ساعت معتبر است.</p>
                           <p>اگر شما این درخواست را نداده‌اید، این ایمیل را نادیده بگیرید.</p>
                       </div>
                       """;

        await emailSender.SendEmailAsync(
            notification.Email,
            "بازیابی رمز عبور",
            body,
            cancellationToken);

        logger.LogInformation(
            "Password reset email sent — UserId: {UserId}, Email: {Email}",
            notification.UserId, notification.Email);
    }
}

#endregion

#region ==================== Send Activation Link on Code Regenerated ====================

/// <summary>
/// Sends activation email when user requests to resend activation link
/// </summary>
public sealed class SendActivationLinkOnActivationCodeRegeneratedDomainEventHandler(
    IEmailSender emailSender,
    ILogger<SendActivationLinkOnActivationCodeRegeneratedDomainEventHandler> logger)
    : IEventHandler<ActivationCodeRegeneratedDomainEvent>
{
    public async Task Handle(
        ActivationCodeRegeneratedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        // Build activation link using the callback URL from the event
        var activationLink  = $"{notification.CallbackUrl}?token={notification.ActiveCode}";


        string body = $"""
                       <div dir="rtl" style="font-family: Tahoma, sans-serif;">
                           <h2>سلام {notification.UserName} عزیز</h2>
                           <p>لینک فعال‌سازی جدید برای حساب کاربری شما:</p>
                           <a href="{activationLink}"
                              style="display:inline-block; padding:12px 24px;
                                     background:#4CAF50; color:white;
                                     text-decoration:none; border-radius:6px;">
                              فعال‌سازی حساب
                           </a>
                           <p>این لینک تا ۲۴ ساعت معتبر است.</p>
                           <p>لینک قبلی دیگر معتبر نیست.</p>
                       </div>
                       """;

        await emailSender.SendEmailAsync(
            notification.Email,
            "ارسال مجدد لینک فعال‌سازی",
            body,
            cancellationToken);

        logger.LogInformation(
            "Activation code regenerated email sent — UserId: {UserId}, Email: {Email}",
            notification.UserId, notification.Email);
    }
}

#endregion
