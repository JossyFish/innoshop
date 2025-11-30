namespace UserService.Application.Interfaces
{
    public interface ILinkTokenGenerator
    {
        string GenerateToken(string email);
        string? ValidateToken(string token);
    }
}