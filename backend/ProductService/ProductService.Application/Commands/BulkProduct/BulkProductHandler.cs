using Auth.Core.Services.Interfaces;
using MediatR;
using ProductService.Domain.Enums;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.BulkProduct
{
    public sealed class BulkProductHandler : IRequestHandler<BulkProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserContext _userContext;

        public BulkProductHandler(IProductRepository productRepository, IUserContext userContext)
        {
            _productRepository = productRepository;
            _userContext = userContext;
        }

        public async Task<Unit> Handle(BulkProductCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();

            var products = await _productRepository.GetByIdsAsync(command.ProductIds, cancellationToken);

            var invalidProducts = products.Where(p => p.UserId != userId).ToList();

            if (invalidProducts.Any())
                throw new ProductAccessDeniedException(invalidProducts.First().Id, userId);

            switch (command.Operation)
            {
                case ProductOperations.Delete:
                    await _productRepository.BulkDeleteAsync(command.ProductIds, cancellationToken);
                    break;

                case ProductOperations.Deactivate:
                    await _productRepository.BulkUpdateActiveAsync(command.ProductIds, false, cancellationToken);
                    break;

                case ProductOperations.Activate:
                    await _productRepository.BulkUpdateActiveAsync(command.ProductIds, true, cancellationToken);
                    break;

                default:
                    throw new InvalidOperation(command.Operation.ToString());
            }

            return Unit.Value;
        }
    }
}
