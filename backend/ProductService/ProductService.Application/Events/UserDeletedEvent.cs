
namespace UserService.Application.Events
{
    public record UserDeletedEvent
    {
        public Guid UserId { get; init; }
    }
}
