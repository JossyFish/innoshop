using Auth.Core.Services.Interfaces;
using MediatR;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;

namespace ProductService.Application.Commands.AddProduct
{
    public sealed class AddProductHandler : IRequestHandler<AddProductCommand, Unit>
    {
        IUserContext _userContext;
        IProductRepository _productRepository;
        public AddProductHandler(IUserContext userContext, IProductRepository productRepository) 
        {
            _userContext = userContext;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(AddProductCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();

            var price = new Price(command.Cost, command.Currency);

            var product = Product.Create(
                Guid.NewGuid(),
                command.Name,
                command.Description,
                price,
                command.Quantity,
                command.IsActive,
                userId,
                DateTime.UtcNow
                );

            await _productRepository.AddAsync(product, cancellationToken);

            return Unit.Value;
        }
    }
}
