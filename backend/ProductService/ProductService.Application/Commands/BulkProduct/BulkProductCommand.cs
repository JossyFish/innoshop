using MediatR;
using ProductService.Domain.Enums;

namespace ProductService.Application.Commands.BulkProduct
{
    public record BulkProductCommand : IRequest<Unit>
    {
        public Guid[] ProductIds { get; set; } = Array.Empty<Guid>();
        public ProductOperations Operation { get; set; }
    }
}
