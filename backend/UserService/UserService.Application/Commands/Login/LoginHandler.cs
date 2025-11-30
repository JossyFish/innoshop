using Auth.Core.Enums;
using Auth.Core.Services.Interfaces;
using MediatR;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.Login
{
    public sealed class LoginHandler  : IRequestHandler<LoginCommand, string>
    {
        IUsersRepository _usersRepository;
        IPasswordHasher _passwordHasher;
        IJwtProvider _jwtProvider;

        public LoginHandler(IUsersRepository usersRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _usersRepository = usersRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }
        public async Task<string> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetByEmailAsync(command.Email, cancellationToken);

            if (user == null) 
                throw new UserNotFoundException(command.Email);

            var compare = _passwordHasher.Verify(command.Password, user.PasswordHash);
            if (!compare)
                throw new InvalidPasswordException(command.Email);
 
            var roles = user.Roles.Select(roleString => (Role)Enum.Parse(typeof(Role), roleString)).ToArray();

            var token = _jwtProvider.GenerateToken(user.Id, roles);

            return token;
        }

    }
}
