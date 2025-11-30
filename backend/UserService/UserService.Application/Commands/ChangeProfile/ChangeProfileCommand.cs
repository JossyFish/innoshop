using MediatR;

namespace UserService.Application.Commands.ChangeProfile
{
    public record ChangeProfileCommand : IRequest<Unit>
    {
        public string NewName { get; set; } = string.Empty;
    }
}
