using MediatR;

namespace UserService.Application.Commands.ConfirmNewPassword
{
    public record ConfirmNewPasswordCommand : IRequest<Unit>
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmationCode { get; set; } = string.Empty;
    }
}
