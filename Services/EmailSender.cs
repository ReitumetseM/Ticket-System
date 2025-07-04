using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
namespace OmnitakSupportHub.Services
{
    public class EmailSender : IEmailSender
        {
            private readonly EmailSettings _settings;
            private readonly ILogger<EmailSender> _logger;

            public EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
            {
                _settings = settings.Value;
                _logger = logger;
            }

            public async Task SendEmailAsync(string email, string subject, string htmlMessage)
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
                    message.To.Add(MailboxAddress.Parse(email));
                    message.Subject = subject;
                    message.Body = new TextPart("html") { Text = htmlMessage };

                    using var client = new SmtpClient();

                    await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_settings.Username, _settings.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    _logger.LogInformation($"Email sent to {email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send email to {email}");
                    throw;
                }
            }
    }
}
