using Auth.Core.Data;
using Auth.Core.Enums;
using Auth.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AuthorizationOptions _authorizationOptions;

        public PermissionService(IOptions<AuthorizationOptions> authorizationOptions)
        {
            _authorizationOptions = authorizationOptions.Value;
        }

        public Task<HashSet<Permission>> GetPermissionsAsync(Role role)
        {
            var rolePermissions = _authorizationOptions.RolePermissions;

            if (rolePermissions == null)
            {
                return Task.FromResult(new HashSet<Permission> { Permission.Read });
            }

            var permissionStrings = rolePermissions
                .FirstOrDefault(rp => rp.Role == role.ToString())?
                .Permissions;

            if (permissionStrings == null)
                return Task.FromResult(new HashSet<Permission> { Permission.Read });

            var permissions = permissionStrings
                .Select(p => Enum.Parse<Permission>(p))
                .ToHashSet();

            return Task.FromResult(permissions);
        }
    }
}
