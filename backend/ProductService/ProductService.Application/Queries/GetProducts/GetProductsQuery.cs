using MediatR;
using ProductService.Domain.Enums;

namespace ProductService.Application.Queries.GetProducts
{
    public class GetProductsQuery : IRequest<GetProductsResponse>
    {
        public int PageNumber {  get; init; }  = 1;
        public int PageSize { get; init; } = 20;
        public string? Name { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public Currency? Currency { get; init; }
        public int? MinQuantity { get; init; }
        public List<Guid>? UserIds { get; init; } = new List<Guid>();
        public DateTime? MinCreatedAt { get; init; }
        public DateTime? MaxCreatedAt { get; init; }
    }
}
