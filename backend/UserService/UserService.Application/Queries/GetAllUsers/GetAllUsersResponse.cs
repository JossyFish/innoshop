namespace UserService.Application.Queries.GetAllUsers
{
    public record GetAllUsersResponse
    (
        Guid Id,
        string Name,
        string Email,
        bool IsActive,
        DateTime CreatedAt,
        List<string> Roles
    );
}
