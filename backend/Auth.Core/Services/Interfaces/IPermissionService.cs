using Auth.Core.Enums;

namespace Auth.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<HashSet<Permission>> GetPermissionsAsync(Role role);
    }
}