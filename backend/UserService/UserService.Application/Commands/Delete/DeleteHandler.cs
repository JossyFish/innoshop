using Auth.Core.Services.Interfaces;
using MassTransit;
using MediatR;
using UserService.Application.Events;
using UserService.Domain.Exceptions;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.Delete
{
    public class DeleteHandler : IRequestHandler<DeleteCommand, Unit>
    {
        IUserContext _userContext;
        IUsersRepository _usersRepository;
        IPublishEndpoint _publishEndpoint;
        public DeleteHandler(IUserContext userContext, IUsersRepository usersRepository, IPublishEndpoint publishEndpoint)
        {
            _userContext = userContext;
            _usersRepository = usersRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(DeleteCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();

            var result = await _usersRepository.DeleteByIdAsync(userId, cancellationToken);
            if (result == false)
                throw new UserNotFoundByIdException(userId);

            await _publishEndpoint.Publish(new UserDeletedEvent
            {
                UserId = userId
            }, cancellationToken);

            return Unit.Value;
        }

    }
}
