using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace API.Services.Implementation
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_config["Gmail:From"]));
            msg.To.Add(MailboxAddress.Parse(email));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = htmlMessage };

            using var smtp = new SmtpClient();
            smtp.CheckCertificateRevocation = false;
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(_config["Gmail:Username"],_config["Gmail:AppPassword"]);

            await smtp.SendAsync(msg);

            await smtp.DisconnectAsync(true);
        }
    }
}
