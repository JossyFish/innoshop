using UserService.Domain.Models;

namespace UserService.Infrastructure.Repositories
{
    public interface IUsersRepository
    {
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task ChangeAsync(User user, CancellationToken cancellationToken);
        Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
        Task<(List<User> Users, int TotalCount)> GetFilterUsersAsync(UserFilters filters, Pagination pagination, CancellationToken cancellationToken);
    }
}