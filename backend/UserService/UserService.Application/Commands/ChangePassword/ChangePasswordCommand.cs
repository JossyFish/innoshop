using MediatR;

namespace UserService.Application.Commands.ChangePassword
{
    public record ChangePasswordCommand : IRequest<Unit>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
