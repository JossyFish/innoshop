using Auth.Core.Services.Interfaces;
using MediatR;
using ProductService.Application.Interfaces;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.ChangeProduct
{
    public class ChangeProductHandler : IRequestHandler<ChangeProductCommand, Unit>
    {
        IProductRepository _productRepository;
        IUserContext _userContext;
        IFindProductService _findProductService;

        public ChangeProductHandler(IProductRepository productRepository, IUserContext userContext, IFindProductService findProductService)
        {
            _productRepository = productRepository;
            _userContext = userContext;
            _findProductService = findProductService;
        }

        public async Task<Unit> Handle(ChangeProductCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();
            var product = await _findProductService.GetProductAsync(command.ProductId, cancellationToken);

            if (product.UserId != userId)
                throw new ProductAccessDeniedException(command.ProductId, userId);

            if (!string.IsNullOrWhiteSpace(command.Name))
                product.ChangeName(command.Name);

            if (!string.IsNullOrWhiteSpace(command.Description))
                product.ChangeDescription(command.Description);

            if (command.Cost.HasValue || command.Currency.HasValue)
            {
                var newCost = command.Cost ?? product.Price.Cost;
                var newCurrency = command.Currency ?? product.Price.Currency;
                product.ChangePrice(newCost, newCurrency);
            }

            if (command.Quantity.HasValue)
                product.ChangeQuantity(command.Quantity.Value);

            if (command.IsActive.HasValue)
                product.ChangeActive(command.IsActive.Value);

            await _productRepository.UpdateAsync(product, cancellationToken);

            return Unit.Value;
        }
    }
}
