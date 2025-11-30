using Auth.Core.Enums;

namespace Auth.Core.Services.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(Guid userId, Role[] roles);
        string GenerateToken(Guid userId, Role role);
    }
}