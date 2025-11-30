using MediatR;

namespace ProductService.Application.Queries.GetMyProducts
{
    public class GetMyProductsQuery : IRequest<GetMyProductsResponse>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
