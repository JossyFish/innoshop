using Auth.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserService.Domain.Entities;
using UserService.Infrastructure.Configurations;

namespace UserService.Infrastructure.Data
{
    public class UserDBContext(DbContextOptions<UserDBContext> options,
            IOptions<AuthorizationOptions> authOptions) : DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDBContext).Assembly);
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration(authOptions.Value));
        }
    }
}
