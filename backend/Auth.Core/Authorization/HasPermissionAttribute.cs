using Auth.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Core.Data
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permission permission) 
                : base(policy: permission.ToString())
        {
        }
    }
}