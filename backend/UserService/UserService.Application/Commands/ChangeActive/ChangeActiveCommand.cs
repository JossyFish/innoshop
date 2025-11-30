using MediatR;

namespace UserService.Application.Commands.ChangeActive
{
    public record ChangeActiveCommand : IRequest<Unit>
    {
        public bool IsActive { get; set; }
    }
}
