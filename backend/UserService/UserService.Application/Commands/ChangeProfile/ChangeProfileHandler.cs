using MediatR;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.ChangeProfile
{
    public class ChangeProfileHandler : IRequestHandler<ChangeProfileCommand, Unit>
    {
        IBaseUserService _baseUserHandler;
        IUsersRepository _usersRepository;
        public ChangeProfileHandler(IBaseUserService baseUserHandler, IUsersRepository usersRepository)
        {
            _baseUserHandler = baseUserHandler;
            _usersRepository = usersRepository;
        }

        public async Task<Unit> Handle(ChangeProfileCommand command, CancellationToken cancellationToken)
        {
            var user = await _baseUserHandler.GetCurrentUserAsync(cancellationToken);

            user.ChangeName(command.NewName);

            await _usersRepository.ChangeAsync(user, cancellationToken);

            return Unit.Value;
        }
    }
    
}
