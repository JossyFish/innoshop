using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Configurations
{
    public partial class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {

        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
              .IsRequired()
              .HasMaxLength(100); 

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255); 

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()"); 

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.HasIndex(x => x.IsActive);

            builder.HasMany(u => u.Roles)
               .WithMany(r => r.Users)
               .UsingEntity<UserRoleEntity>(
               l => l.HasOne<RoleEntity>().WithMany().HasForeignKey(r => r.RoleId)
                   .OnDelete(DeleteBehavior.Cascade), 
               r => r.HasOne<UserEntity>().WithMany().HasForeignKey(u => u.UserId)
                   .OnDelete(DeleteBehavior.Cascade));

        }
    }
}
