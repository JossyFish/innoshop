using MediatR;

namespace UserService.Application.Commands.Login
{
    public record LoginCommand : IRequest<string>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
