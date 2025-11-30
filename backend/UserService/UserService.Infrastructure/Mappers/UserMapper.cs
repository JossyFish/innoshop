using UserService.Domain.Entities;
using UserService.Domain.Models;

namespace UserService.Infrastructure.Mappers
{
    public static class UserMapper
    {
        public static User MapToModel(UserEntity entity)
        {
            var roles = entity.Roles?
              .Select(r => r.Name)
              .ToList() ?? new List<string>();

            return User.CreateWithRoles(
                entity.Id,
                entity.Name,
                entity.Email,
                entity.PasswordHash,
                entity.IsActive,
                entity.CreatedAt,
                roles
            );
        }
    }
}
