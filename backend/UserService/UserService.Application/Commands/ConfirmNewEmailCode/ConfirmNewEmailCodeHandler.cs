using MediatR;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.ConfirmNewEmailCode
{
    public class ConfirmNewEmailCodeHandler : IRequestHandler<ConfirmNewEmailCodeCommand, Unit>
    {
        IBaseUserService _baseUserHandler;
        ICacheUsersRepository _cache;
        IUsersRepository _usersRepository;
        public ConfirmNewEmailCodeHandler(IBaseUserService baseUserHandler, ICacheUsersRepository cache,
            IUsersRepository usersRepository)
        {
            _baseUserHandler = baseUserHandler;
            _cache = cache;
            _usersRepository = usersRepository;
        }

        public async Task<Unit> Handle(ConfirmNewEmailCodeCommand command, CancellationToken cancellationToken)
        {
            var user = await _baseUserHandler.GetCurrentUserAsync(cancellationToken);

            var newEmailCache = await _cache.GetChangeEmailDataAsync(user.Id, cancellationToken);

            if (newEmailCache == null)
                throw new CacheDataNotFoundException("Confirm new Email", user.Email);

            if (newEmailCache.ConfirmationCode != command.ConfirmationCode)
                throw new InvalidConfirmCodeException(user.Email);

            var existingUser = await _usersRepository.GetByEmailAsync(newEmailCache.NewEmail, cancellationToken);
            if (existingUser != null) 
                throw new UserAlreadyExistException(newEmailCache.NewEmail);

            user.ChangeEmail(newEmailCache.NewEmail);

            await _usersRepository.ChangeAsync(user, cancellationToken);
            await _cache.RemoveChangeEmailDataAsync(user.Id, cancellationToken);

            return Unit.Value;
        }

    }
}
