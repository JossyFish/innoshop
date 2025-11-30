using MediatR;
using ProductService.Domain.Enums;

namespace ProductService.Application.Commands.ChangeProduct
{
    public record ChangeProductCommand : IRequest<Unit>
    {
        public Guid ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Cost { get; set; }
        public Currency? Currency { get; set; }
        public int? Quantity { get; set; }
        public bool? IsActive { get; set; }
    }
}
