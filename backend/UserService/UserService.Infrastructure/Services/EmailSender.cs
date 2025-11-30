using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using UserService.Domain.Models;
using UserService.Domain.Interfaces;

namespace UserService.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions _options;
        ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailOptions> options, ILogger<EmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync (Email email, CancellationToken cancellationToken)
        {
            try
            {
                using var smtpClient = new SmtpClient(_options.SmtpServer, _options.Port)
                {
                    EnableSsl = _options.EnableSsl,
                    Credentials = new NetworkCredential(_options.Username, _options.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 30000
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_options.FromEmail, _options.FromName),
                    Subject = email.Subject,
                    Body = email.Body,
                    IsBodyHtml = email.IsHtml
                };

                mailMessage.To.Add(email.UserEmail);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email: {email}", email.UserEmail);
            }
        }
    }
}
