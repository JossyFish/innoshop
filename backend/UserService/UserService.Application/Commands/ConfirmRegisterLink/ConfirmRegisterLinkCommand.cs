using MediatR;

namespace UserService.Application.Commands.ConfirmRegisterLink
{
    public record ConfirmRegisterLinkCommand(string Token) : IRequest<string>
    { 
    }
}
