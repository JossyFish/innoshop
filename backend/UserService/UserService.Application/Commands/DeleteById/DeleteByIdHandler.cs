using MassTransit;
using MediatR;
using UserService.Application.Events;
using UserService.Domain.Exceptions;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.DeleteById
{
    public class DeleteByIdHandler : IRequestHandler<DeleteByIdCommand, Unit>
    {
        IUsersRepository _usersRepository;
        IPublishEndpoint _publishEndpoint;
        public DeleteByIdHandler(IUsersRepository usersRepository, IPublishEndpoint publishEndpoint)
        {
            _usersRepository = usersRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(DeleteByIdCommand command, CancellationToken cancellationToken)
        { 

            var result = await _usersRepository.DeleteByIdAsync(command.Id, cancellationToken);
            if (result == false)
                throw new UserNotFoundByIdException(command.Id);

            await _publishEndpoint.Publish(new UserDeletedEvent
            {
                UserId = command.Id
            }, cancellationToken);


            return Unit.Value;
        }

    }
}
