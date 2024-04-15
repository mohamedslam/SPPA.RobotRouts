using MailKit.Net.Smtp;
using SPPA.Domain;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SPPA.Logic.Services;

public class EmailService
{

    private readonly EmailSettings _settings;

    public EmailService(
        IOptions<AppSettings> appSettings
    )
    {
        _settings = appSettings.Value.EmailNotification!;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("SPPA bot", _settings.Address));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort!.Value, true);
            await client.AuthenticateAsync(_settings.Login, _settings.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}