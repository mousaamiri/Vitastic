namespace Vitastic.App.Common.Abstractions.Services.Base;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
