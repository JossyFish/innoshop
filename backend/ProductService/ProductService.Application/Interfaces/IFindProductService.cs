using ProductService.Domain.Models;

namespace ProductService.Application.Interfaces
{
    public interface IFindProductService
    {
        Task<Product> GetProductAsync(Guid productId, CancellationToken cancellationToken);
    }
}