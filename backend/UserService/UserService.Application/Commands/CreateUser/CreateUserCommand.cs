using MediatR;

namespace UserService.Application.Commands.CreateUser
{
    public record CreateUserCommand : IRequest<Unit>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}
