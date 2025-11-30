using Auth.Core.Data;
using Auth.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using UserService.Domain.Entities;


namespace UserService.Infrastructure.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermissionEntity>
    {
        private readonly AuthorizationOptions _authorization;
        public RolePermissionConfiguration(AuthorizationOptions authorization)
        {
            _authorization = authorization;
        }
        public void Configure(EntityTypeBuilder<RolePermissionEntity> builder)
        {
            builder.HasKey(r => new { r.RoleId, r.PermissionId });

            builder.HasData(ParseRolePermissions());
        }

        private RolePermissionEntity[] ParseRolePermissions()
        {
            return _authorization.RolePermissions
                .SelectMany(rp => rp.Permissions
                    .Select(p  => new RolePermissionEntity
                    {
                        RoleId = (int)Enum.Parse<Role>(rp.Role),
                        PermissionId = (int)Enum.Parse<Permission>(p)
                    }))
                    .ToArray();
        }
    }
}
