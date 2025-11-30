using MassTransit;
using MediatR;
using UserService.Application.Events;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.ChangeActive
{
    public class ChangeActiveHandler : IRequestHandler<ChangeActiveCommand, Unit>
    {
        IBaseUserService _baseUserHandler;
        IUsersRepository _usersRepository;
        IPublishEndpoint _publishEndpoint;
        public ChangeActiveHandler(IBaseUserService baseUserHandler, IUsersRepository usersRepository, IPublishEndpoint publishEndpoint)
        {
            _baseUserHandler = baseUserHandler;
            _usersRepository = usersRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(ChangeActiveCommand command, CancellationToken cancellationToken)
        {
            var user = await _baseUserHandler.GetCurrentUserAsync(cancellationToken);

            user.ChangeActive(command.IsActive);

            await _usersRepository.ChangeAsync(user, cancellationToken);

            if (!command.IsActive)
            {
                await _publishEndpoint.Publish(new UserDeactivatedEvent
                {
                    UserId = user.Id,
                }, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
