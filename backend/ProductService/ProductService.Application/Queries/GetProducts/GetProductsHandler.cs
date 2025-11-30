using MediatR;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;

namespace ProductService.Application.Queries.GetProducts
{
    public sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, GetProductsResponse>
    {
        IProductRepository _productRepository;
        public GetProductsHandler(IProductRepository productRepository)    
        {
            _productRepository = productRepository;
        }

        public async Task<GetProductsResponse> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var pageNumber = Math.Max(1, query.PageNumber);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);

            var filters = new Filters
            {
                Name = query.Name,
                MinPrice = query.MinPrice,
                MaxPrice = query.MaxPrice,
                Currency = query.Currency,
                MinQuantity = query.MinQuantity,
                UserIds = query.UserIds?.ToList(),
                MinCreatedAt = query.MinCreatedAt,
                MaxCreatedAt = query.MaxCreatedAt
            };

            var pagination = new Pagination
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var (products, totalCount) = await _productRepository.GetFilteredProductsAsync(
               filters,
               pagination,
               cancellationToken
           );

            var productDtos = products.Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Quantity,
                p.IsActive,
                p.UserId,
                p.CreatedAt
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new GetProductsResponse(
                productDtos,
                totalCount,
                pageNumber,
                pageSize,
                totalPages
            );
        }
    }
}
