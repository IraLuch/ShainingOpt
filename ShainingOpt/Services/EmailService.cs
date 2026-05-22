using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using ShainingOpt.Services.Configurations;

namespace ShainingOpt.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _options;

        public EmailService(IOptions<SmtpSettings> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string mess)
        {
            using var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Администрация сайта", _options.SenderEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = mess
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_options.Server, _options.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_options.Username, _options.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
