using Auth.Core.Enums;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Exceptions;
using UserService.Domain.Models;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Mappers;

namespace UserService.Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private UserDBContext _context;
        public UsersRepository(UserDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            var roleEntity = await _context.Roles
             .SingleOrDefaultAsync(r => r.Id == (int)Auth.Core.Enums.Role.User, cancellationToken);

            if (roleEntity == null)
                throw new InvalidOperationException("User role not found");

            var userEntity = new UserEntity()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = [roleEntity]
            };

            await _context.Users.AddAsync(userEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeAsync(User user, CancellationToken cancellationToken)
        {
            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

            if (userEntity == null)
                throw new UserNotFoundByIdException(user.Id); 

            userEntity.Name = user.Name;
            userEntity.Email = user.Email;
            userEntity.PasswordHash = user.PasswordHash;
            userEntity.IsActive = user.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (userEntity == null)
                return false;

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync(cancellationToken); 
            return true;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var userEntity = await _context.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (userEntity == null)
            { return null; }

            return UserMapper.MapToModel(userEntity);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var userEntity = await _context.Users
               .AsNoTracking()
               .Include(u => u.Roles)
               .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (userEntity == null)
            { return null; }

            return UserMapper.MapToModel(userEntity);
        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            var userEntities = await _context.Users
              .AsNoTracking()
              .Include(u => u.Roles)
              .ToListAsync(cancellationToken);

            return userEntities.Select(UserMapper.MapToModel).ToList();
        }

        public async Task<(List<User> Users, int TotalCount)> GetFilterUsersAsync(UserFilters filters, Pagination pagination, CancellationToken cancellationToken)
        {
           var query = _context.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Search))
                query = query.Where(u => u.Name.Contains(filters.Search));

            if (filters.IsActive.HasValue)
                query = query.Where(u => u.IsActive == filters.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(filters.RoleName))
                query = query.Where(u => u.Roles.Any(r => r.Name == filters.RoleName));

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderBy(u => u.Name) 
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync(cancellationToken);

            return (users.Select(UserMapper.MapToModel).ToList(), totalCount);
        }

    }
}
