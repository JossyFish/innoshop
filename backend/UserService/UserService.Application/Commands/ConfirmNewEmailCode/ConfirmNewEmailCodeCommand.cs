using MediatR;

namespace UserService.Application.Commands.ConfirmNewEmailCode
{
    public record ConfirmNewEmailCodeCommand : IRequest<Unit>
    {
        public string ConfirmationCode { get; set; } = string.Empty;
    }
}
