using MediatR;

namespace UserService.Application.Commands.ChangeEmail
{
    public record ChangeEmailCommand : IRequest<Unit>
    {
        public string NewEmail { get; set; } = string.Empty;
    }
}
