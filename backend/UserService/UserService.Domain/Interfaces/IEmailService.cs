namespace UserService.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string email, string confirmCode, string linkToken, CancellationToken cancellationToken);
        Task SendResetPasswordAsync(string email, string resetPasswordCode, CancellationToken cancellationToken);
    }
}