namespace UserService.Application.Queries.GetCurrentUser
{
   public record GetCurrentUserResponse
    (
        Guid Id,
        string Name,
        string Email,
        bool IsActive,
        DateTime CreatedAt,
        List<string> Roles
    );
}
