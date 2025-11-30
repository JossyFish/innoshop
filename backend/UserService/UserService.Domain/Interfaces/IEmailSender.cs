using UserService.Domain.Models;

namespace UserService.Domain.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Email email, CancellationToken cancellationToken);
    }
}