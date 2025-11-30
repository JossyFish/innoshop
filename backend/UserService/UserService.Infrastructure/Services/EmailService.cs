using UserService.Domain.Interfaces;
using UserService.Domain.Models;

namespace UserService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplates _emailTemplates;
       
        public EmailService(IEmailSender emailSender, IEmailTemplates emailTemplates)
        {
            _emailSender = emailSender;
            _emailTemplates = emailTemplates;
        }

        public async Task SendConfirmationEmailAsync(string email, string confirmCode, string link, CancellationToken cancellationToken)
        {
            var emailMessage = new Email(
                userEmail: email,
                subject: "Код подтверждения InnoShop",
                body: _emailTemplates.CreateConfirmationEmail(confirmCode, link),
                isHtml: true
            );
            await _emailSender.SendEmailAsync(emailMessage, cancellationToken);
        }

        public async Task SendResetPasswordAsync(string email, string resetPasswordCode, CancellationToken cancellationToken)
        {
            var emailMessage = new Email(
                userEmail: email,
                subject: "Код сброса пароля InnoShop",
                body: _emailTemplates.CreateResetPasswordEmail(resetPasswordCode),
                isHtml: true
            );
            await _emailSender.SendEmailAsync(emailMessage, cancellationToken);
        }

    }
}
