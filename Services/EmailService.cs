using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Mail;

namespace OmnitakSupportHub.Services
{
    public class EmailService
    {
       
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        private readonly string senderEmail = "rmmosehla23@gmail.com";
        private readonly string senderName = "OmnitakSupportHub";
        private readonly string smtpUsername = "rmmosehla23@gmail.com";
        private readonly string smtpPassword = "xsjo ybmr zumf rogu";

        public void SendRegistrationEmail(string toEmail, string toName)
        {
            var email = new MimeMessage();

            // Use private attributes instead of inline MailboxAddress
            email.From.Add(new MailboxAddress(senderName, senderEmail));
            email.To.Add(new MailboxAddress(toName, toEmail));

            email.Subject = "Welcome to Omnitak Support Hub!";
            email.Body = new TextPart("html")
            {
                Text = $"<h2>Hello {toName}!</h2><p>Thank you for registering with us.</p>"
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect(smtpServer, smtpPort, false);
                smtp.Authenticate(smtpUsername, smtpPassword);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
