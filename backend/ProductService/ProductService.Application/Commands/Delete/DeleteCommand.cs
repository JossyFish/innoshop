using MediatR;

namespace ProductService.Application.Commands.Delete
{
    public record DeleteCommand : IRequest<Unit>
    {
        public Guid ProductId { get; set; } 
    }
}
