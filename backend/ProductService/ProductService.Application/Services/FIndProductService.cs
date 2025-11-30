using ProductService.Application.Interfaces;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;

namespace ProductService.Application.Services
{
    public class FIndProductService : IFindProductService
    {
        IProductRepository _productRepository;
        public FIndProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> GetProductAsync(Guid productId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);

            if (product == null)
                throw new ProductNotFoundException(productId);

            return product;
        }
    }
}
