using MediatR;

namespace UserService.Application.Commands.Delete
{
    public record DeleteCommand : IRequest<Unit>
    {
    }
}
