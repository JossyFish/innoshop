using ProductService.Domain.Models;

namespace ProductService.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product, CancellationToken cancellationToken);
        Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
        Task<int> GetTotalProductsCountAsync(CancellationToken cancellationToken);
        Task<(List<Product> Products, int TotalCount)> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);
        Task<(List<Product> Products, int TotalCount)> GetFilteredProductsAsync(Filters filters, Pagination pagination,
        CancellationToken cancellationToken);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Product>> GetByIdsAsync(Guid[] productIds, CancellationToken cancellationToken);
        Task UpdateAsync(Product product, CancellationToken cancellationToken);
        Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteAllAsync(Guid userId, CancellationToken cancellationToken);
        Task DeactivateAllAsync(Guid userId, CancellationToken cancellationToken);
        Task BulkDeleteAsync(Guid[] productIds, CancellationToken cancellationToken);
        Task BulkUpdateActiveAsync(Guid[] productIds, bool isActive, CancellationToken cancellationToken);
    }
}