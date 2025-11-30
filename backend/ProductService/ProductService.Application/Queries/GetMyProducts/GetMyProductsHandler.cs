using Auth.Core.Services.Interfaces;
using MediatR;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Queries.GetMyProducts
{
    public class GetMyProductsHandler : IRequestHandler<GetMyProductsQuery, GetMyProductsResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserContext _userContext;

        public GetMyProductsHandler(IProductRepository productRepository, IUserContext userContext)
        {
            _productRepository = productRepository;
            _userContext = userContext;
        }

        public async Task<GetMyProductsResponse> Handle(GetMyProductsQuery query, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();
            var pageNumber = Math.Max(1, query.PageNumber);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);

            var (products, totalCount) = await _productRepository.GetByUserIdAsync(userId, pageNumber, pageSize, cancellationToken);

            var productDtos = products.Select(p => new MyProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Quantity,
                p.IsActive,
                p.CreatedAt
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new GetMyProductsResponse(
                productDtos,
                totalCount,
                pageNumber,
                pageSize,
                totalPages
            );
        }
    }
}
