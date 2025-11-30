using MediatR;

namespace UserService.Application.Queries.GetSellers
{
    public class GetSellersQuery : IRequest<GetSellersResponse>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public string? Search { get; init; }
    }
}
