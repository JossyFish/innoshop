using MediatR;

namespace ProductService.Application.Queries.GetById
{
    public class GetByIdQuery : IRequest<GetByIdResponse>
    {
        public Guid Id { get; set; }
    }
}
