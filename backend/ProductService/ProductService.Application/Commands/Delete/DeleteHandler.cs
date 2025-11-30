using Auth.Core.Services.Interfaces;
using MediatR;
using ProductService.Application.Interfaces;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.Delete
{
    public sealed class DeleteHandler : IRequestHandler<DeleteCommand, Unit>
    {
        IProductRepository _productRepository;
        IUserContext _userContext;
        IFindProductService _findProductService;

        public DeleteHandler(IProductRepository productRepository, IUserContext userContext, IFindProductService findProductService)
        {
            _productRepository = productRepository;
            _userContext = userContext;
            _findProductService = findProductService;
        }

        public async Task<Unit> Handle(DeleteCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();

            var product = await _findProductService.GetProductAsync(command.ProductId, cancellationToken);

            if (product.UserId != userId)
                throw new ProductAccessDeniedException(command.ProductId, userId);

            await _productRepository.DeleteByIdAsync(command.ProductId, cancellationToken);

            return Unit.Value;
        }

    }   
}
