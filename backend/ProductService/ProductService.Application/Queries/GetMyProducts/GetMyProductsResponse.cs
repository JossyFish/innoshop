using ProductService.Domain.ValueObjects;

namespace ProductService.Application.Queries.GetMyProducts
{
    public record GetMyProductsResponse
    (
        List<MyProductResponse> Products,
        int TotalCount,
        int PageNumber,
        int PageSize,
        int TotalPages
    );
    public record MyProductResponse(
        Guid Id,
        string Name,
        string Description,
        Price Price,
        int Quantity,
        bool IsActive,
        DateTime CreatedAt
    );
}
