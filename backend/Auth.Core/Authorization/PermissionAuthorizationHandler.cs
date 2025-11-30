using Auth.Core.Enums;
using Auth.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Security.Claims;

namespace Auth.Core.Data
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        IServiceScopeFactory _serviceScopeFactory;
        public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory) { 
        _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (!userRoles.Any())
            {
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();

            var permissionService =scope.ServiceProvider.GetRequiredService<IPermissionService>();

            var userPermissions = new HashSet<Permission>();

            foreach (var role in userRoles)
            {
                if (Enum.TryParse<Role>(role, out var roleNum))
                {
                    var rolePermissions = await permissionService.GetPermissionsAsync(roleNum);
                    userPermissions.UnionWith(rolePermissions);
                }
            }
    
            if (userPermissions.Intersect(requirement.Permissions).Any()) {
                context.Succeed(requirement);
            }
        }
    }
}
