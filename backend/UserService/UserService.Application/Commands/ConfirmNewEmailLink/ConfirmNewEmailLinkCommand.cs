using MediatR;

namespace UserService.Application.Commands.ConfirmNewEmailLink
{
    public record ConfirmNewEmailLinkCommand(string Token) : IRequest<Unit>
    {
  
    }
}
