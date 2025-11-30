namespace Auth.Core.Services.Interfaces
{
    public interface IUserContext
    {
        Guid GetCurrentUserId();
    }
}