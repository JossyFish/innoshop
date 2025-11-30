using ProductService.Domain.Entities;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Mappers
{
    public static class ProductMapper
    {
        public static Product MapToModel(ProductEntity entity)
        {
            return Product.Create(
                entity.Id,
                entity.Name,
                entity.Description,
                entity.Price,
                entity.Quantity,
                entity.IsActive,
                entity.UserId,
                entity.CreatedAt
            );
        }
    }
}
