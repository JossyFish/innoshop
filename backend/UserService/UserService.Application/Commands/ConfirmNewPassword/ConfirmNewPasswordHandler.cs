using MediatR;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.ConfirmNewPassword
{
    public sealed class ConfirmNewPasswordHandler : IRequestHandler<ConfirmNewPasswordCommand, Unit>
    {
        ICacheUsersRepository _cache;
        IPasswordHasher _passwordHasher;
        IUsersRepository _usersRepository;
        public ConfirmNewPasswordHandler(ICacheUsersRepository cache, IPasswordHasher passwordHasher, IUsersRepository usersRepository)
        {
            _cache = cache;
            _passwordHasher = passwordHasher;
            _usersRepository = usersRepository;
        }

        public async Task<Unit> Handle(ConfirmNewPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetByEmailAsync(command.Email, cancellationToken);

            if (user == null)
                throw new UserNotFoundException(command.Email);

            var resetPasswordCache = await _cache.GetResetPasswordDataAsync(user.Id, cancellationToken);

            if (resetPasswordCache == null)
                throw new CacheDataNotFoundException("ResetPassword", command.Email);

            if (resetPasswordCache.ConfirmationCode != command.ConfirmationCode)
                throw new InvalidConfirmCodeException(command.Email);
           
            await _cache.RemoveResetPasswordDataAsync(user.Id, cancellationToken);

            var newHashedPassword = _passwordHasher.Generate(command.NewPassword);

            user.ChangePassword(newHashedPassword);

            await _usersRepository.ChangeAsync(user, cancellationToken);

            return Unit.Value;
        }
    }
}
