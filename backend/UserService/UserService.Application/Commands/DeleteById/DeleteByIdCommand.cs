using MediatR;

namespace UserService.Application.Commands.DeleteById
{
    public record DeleteByIdCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
