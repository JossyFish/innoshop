using ProductService.Domain.ValueObjects;

namespace ProductService.Application.Queries.GetById
{
    public record GetByIdResponse
    (
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
