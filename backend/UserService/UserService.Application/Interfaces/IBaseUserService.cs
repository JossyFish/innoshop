using UserService.Domain.Models;

namespace UserService.Application.Interfaces
{
    public interface IBaseUserService
    {
        Task<User> GetCurrentUserAsync(CancellationToken cancellationToken);
    }
}