using MediatR;

namespace UserService.Application.Commands.ConfirmRegisterCode
{
    public record ConfirmRegisterCodeCommand : IRequest<string>
    {
        public string Email { get; set; } = string.Empty;
        public string ConfirmationCode { get; set; } = string.Empty;
    }
}
