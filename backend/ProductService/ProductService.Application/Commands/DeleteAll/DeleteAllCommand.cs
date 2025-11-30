using MediatR;

namespace ProductService.Application.Commands.DeleteAll
{
    public record DeleteAllCommand : IRequest<Unit>
    {
    }
}
