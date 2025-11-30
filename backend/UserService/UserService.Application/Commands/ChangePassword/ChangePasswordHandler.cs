using MediatR;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Unit>
    {
        IBaseUserService _baseUserHandler;
        IPasswordHasher _passwordHasher;
        IUsersRepository _usersRepository;
        public ChangePasswordHandler(IBaseUserService baseUserHandler, IPasswordHasher passwordHasher, IUsersRepository usersRepository) 
        {
            _baseUserHandler = baseUserHandler;
            _passwordHasher = passwordHasher;
            _usersRepository = usersRepository;
        }

        public async Task<Unit> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _baseUserHandler.GetCurrentUserAsync(cancellationToken);

            var compare = _passwordHasher.Verify(command.CurrentPassword, user.PasswordHash);
            if (compare == false) 
                throw new InvalidPasswordException(command.NewPassword);

            var newHashedPassword = _passwordHasher.Generate(command.NewPassword);

            user.ChangePassword(newHashedPassword);

            await _usersRepository.ChangeAsync(user, cancellationToken);

            return Unit.Value;
        }
    }
}
