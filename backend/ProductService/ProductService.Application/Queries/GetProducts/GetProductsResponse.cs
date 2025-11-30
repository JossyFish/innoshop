using ProductService.Domain.ValueObjects;

namespace ProductService.Application.Queries.GetProducts
{
    public record GetProductsResponse(
         List<ProductResponse> Products,
         int TotalCount,
         int PageNumber,
         int PageSize,
         int TotalPages
     );

    public record ProductResponse(
         Guid Id,
         string Name,
         string Description,
         Price Price,
         int Quantity,
         bool IsActive,
         Guid UserId,
         DateTime CreatedAt
     );
}
