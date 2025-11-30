using MediatR;
using ProductService.Domain.Enums;

namespace ProductService.Application.Commands.AddProduct
{
    public record AddProductCommand : IRequest<Unit>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public Currency Currency { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
