using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace ColatonDeMelon.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage)
        {
            var gmailEmail = _configuration["Gmail:Email"];
            var gmailPassword = _configuration["Gmail:AppPassword"];

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    "Colatón de Melón",
                    gmailEmail!));

            message.To.Add(
                new MailboxAddress(
                    email,
                    email));

            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = htmlMessage
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                gmailEmail!,
                gmailPassword!);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}