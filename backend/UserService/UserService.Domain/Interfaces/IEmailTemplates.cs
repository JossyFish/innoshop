namespace UserService.Domain.Interfaces
{
    public interface IEmailTemplates
    {
        string CreateConfirmationEmail(string code, string link);
        string CreateResetPasswordEmail(string code);
    }
}