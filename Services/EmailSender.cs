using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using ThienAnFuni.Configurations;

namespace ThienAnFuni.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(
                    _emailSettings.SenderEmail,
                    _emailSettings.SenderPassword
                );

                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; // Thêm dòng này

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Lỗi khi gửi email: {ex.Message} - {ex.StatusCode}");
                    Console.WriteLine($"Chi tiết lỗi: {ex.InnerException?.Message}");
                    throw;
                }

            }
        }

    }
}
