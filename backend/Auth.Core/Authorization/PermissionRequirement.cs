using Auth.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Core.Data
{
    public class PermissionRequirement(Permission[] permissions) : IAuthorizationRequirement
    {
        public Permission[] Permissions { get; set; } = permissions;
    }
}
