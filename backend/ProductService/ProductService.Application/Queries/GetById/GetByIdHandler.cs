using MediatR;
using ProductService.Application.Interfaces;
using ProductService.Domain.Exceptions;

namespace ProductService.Application.Queries.GetById
{
    public sealed class GetByIdHandler : IRequestHandler<GetByIdQuery, GetByIdResponse>
    {
        IFindProductService _findProductService;
        public GetByIdHandler(IFindProductService findProductService)
        {
            _findProductService = findProductService;
        }

        public async Task<GetByIdResponse> Handle(GetByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await _findProductService.GetProductAsync(query.Id, cancellationToken);

            if(product == null)
            {
                throw new ProductNotFoundException(query.Id);
            }

            return new GetByIdResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Quantity,
                product.IsActive,
                product.UserId,
                product.CreatedAt
                );
        }

    }
}
