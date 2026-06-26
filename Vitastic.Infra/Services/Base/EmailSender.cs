using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Vitastic.App.Common.Abstractions.Services.Base;

namespace Vitastic.Infra.Services.Base;

public class EmailSender : IEmailSender
{
    private readonly SmtpClient _smtpClient;
    private readonly string _from;

    public EmailSender(IConfiguration configuration)
    {
        var smtpSection = configuration.GetSection("Smtp");
        _from = smtpSection["From"] ?? throw new ArgumentNullException($"Smtp:From");
        _smtpClient = new SmtpClient
        {
            Host = smtpSection["Host"] ?? throw new ArgumentNullException($"Smtp:Host"),
            Port = int.Parse(smtpSection[$"Port"] ?? "25"),
            EnableSsl = bool.Parse(smtpSection["EnableSsl"] ?? "true"),
            Credentials = new System.Net.NetworkCredential(
                smtpSection["Username"] ?? throw new ArgumentNullException("Smtp:Username"),
                smtpSection["Password"] ?? throw new ArgumentNullException("Smtp:Password"))
        };
    }

    public async Task SendEmailAsync(string to, string subject, string body,
        CancellationToken cancellationToken = default)
    {
        var mail = new MailMessage(_from, to, subject, body);
        mail.IsBodyHtml = true;
        await _smtpClient.SendMailAsync(mail, cancellationToken);
    }
}
