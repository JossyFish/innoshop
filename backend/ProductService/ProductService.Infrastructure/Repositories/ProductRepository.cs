using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Mappers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private ProductDBContext _context;
        public ProductRepository(ProductDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            var productEntity = new ProductEntity()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                IsActive = product.IsActive,
                UserId = product.UserId,
                CreatedAt = product.CreatedAt
            };

            await _context.Products.AddAsync(productEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
        {
            var productEntities = await _context.Products
              .Where(p => p.IsActive)
              .AsNoTracking()
              .ToListAsync(cancellationToken);

            return productEntities.Select(ProductMapper.MapToModel).ToList();
        }

        public async Task<int> GetTotalProductsCountAsync(CancellationToken cancellationToken)
        {
            var totalCount = await _context.Products
                .Where(p => p.IsActive)
                .CountAsync(cancellationToken);

            return totalCount;
        }

        public async Task<(List<Product> Products, int TotalCount)> GetFilteredProductsAsync(Filters filters, Pagination pagination,
                                                                 CancellationToken cancellationToken)
        {
            var productsQuery = _context.Products
              .Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(filters.Name))
                productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(filters.Name.ToLower()));

            if (filters.Currency.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price.Currency == filters.Currency.Value);

                if (filters.MinPrice.HasValue)
                    productsQuery = productsQuery.Where(p => p.Price.Cost >= filters.MinPrice.Value);

                if (filters.MaxPrice.HasValue)
                    productsQuery = productsQuery.Where(p => p.Price.Cost <= filters.MaxPrice.Value);
            }
            else
            {
                if (filters.MinPrice.HasValue)
                    productsQuery = productsQuery.Where(p => p.Price.Cost >= filters.MinPrice.Value);

                if (filters.MaxPrice.HasValue)
                    productsQuery = productsQuery.Where(p => p.Price.Cost <= filters.MaxPrice.Value);
            }

            if (filters.MinQuantity.HasValue)
                productsQuery = productsQuery.Where(p => p.Quantity >= filters.MinQuantity.Value);

            if (filters.UserIds != null && filters.UserIds.Any())
                productsQuery = productsQuery.Where(p => filters.UserIds.Contains(p.UserId));

            if (filters.MinCreatedAt.HasValue)
            {
                var minCreatedAtUtc = filters.MinCreatedAt.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(filters.MinCreatedAt.Value, DateTimeKind.Utc)
                    : filters.MinCreatedAt.Value.ToUniversalTime();
                productsQuery = productsQuery.Where(p => p.CreatedAt >= minCreatedAtUtc);
            }

            if (filters.MaxCreatedAt.HasValue)
            {
                var maxCreatedAtUtc = filters.MaxCreatedAt.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(filters.MaxCreatedAt.Value, DateTimeKind.Utc)
                    : filters.MaxCreatedAt.Value.ToUniversalTime();
                productsQuery = productsQuery.Where(p => p.CreatedAt <= maxCreatedAtUtc);
            }

            var totalCount = await productsQuery.CountAsync(cancellationToken);

            var productEntities = await productsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var products = productEntities.Select(ProductMapper.MapToModel).ToList();

            return (products, totalCount);
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var productEntity = await _context.Products
                .Where(p => p.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken); 

            return productEntity != null ? ProductMapper.MapToModel(productEntity) : null;
        }

        public async Task<List<Product>> GetByIdsAsync(Guid[] productIds, CancellationToken cancellationToken)
        {
            var productEntities =  await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            return productEntities.Select(ProductMapper.MapToModel).ToList();
        }

        public async Task<(List<Product> Products, int TotalCount)> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize,
            CancellationToken cancellationToken)
        {
            var productsQuery = _context.Products
                .Where(p => p.UserId == userId);

            var totalCount = await productsQuery.CountAsync(cancellationToken);

            var productEntities = await productsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var products = productEntities.Select(ProductMapper.MapToModel).ToList();

            return (products, totalCount);
        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            var productEntity = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

            if (productEntity == null)
                throw new ProductNotFoundException(product.Id); 

            productEntity.Name = product.Name;
            productEntity.Description = product.Description;
            productEntity.Price = product.Price;
            productEntity.Quantity = product.Quantity;
            productEntity.IsActive = product.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var productEntity = await _context.Products
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (productEntity == null)
                throw new ProductNotFoundException(id);

            _context.Products.Remove(productEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAllAsync(Guid userId, CancellationToken cancellationToken)
        {
            var productsEntities = await _context.Products
              .Where(p => p.UserId == userId)
              .ToListAsync(cancellationToken);

            if (productsEntities.Any())
            {
                _context.Products.RemoveRange(productsEntities);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeactivateAllAsync(Guid userId, CancellationToken cancellationToken)
        {
            var productsEntities = await _context.Products
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);

            foreach (var product in productsEntities)
            {
                product.IsActive = false;
            }

            if (productsEntities.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task BulkDeleteAsync(Guid[] productIds, CancellationToken cancellationToken) 
        {
            await _context.Products
             .Where(p => productIds.Contains(p.Id))
             .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task BulkUpdateActiveAsync(Guid[] productIds, bool isActive, CancellationToken cancellationToken)
        {
            await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ExecuteUpdateAsync(p =>
                    p.SetProperty(i => i.IsActive, isActive),
                    cancellationToken);
        }

    }
}
