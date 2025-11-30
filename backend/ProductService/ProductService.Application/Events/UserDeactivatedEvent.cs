namespace UserService.Application.Events
{
    public class UserDeactivatedEvent
    {
        public Guid UserId { get; init; }
    }
}
