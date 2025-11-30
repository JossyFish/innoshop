using MediatR;

namespace UserService.Application.Commands.ResetPassword
{
    public record ResetPasswordCommand : IRequest<Unit>
    {
        public string Email { get; set; } = string.Empty;
    }
}
