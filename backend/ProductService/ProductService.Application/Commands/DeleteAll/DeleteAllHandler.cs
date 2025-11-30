using Auth.Core.Services.Interfaces;
using MediatR;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.DeleteAll
{
    public sealed class DeleteAllHandler : IRequestHandler<DeleteAllCommand, Unit>
    {
        IProductRepository _productRepository;
        IUserContext _userContext;

        public DeleteAllHandler(IProductRepository productRepository, IUserContext userContext)
        {
            _productRepository = productRepository;
            _userContext = userContext;
        }

        public async Task<Unit> Handle(DeleteAllCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();

            await _productRepository.DeleteAllAsync(userId, cancellationToken);

            return Unit.Value;
        }

    }
}
