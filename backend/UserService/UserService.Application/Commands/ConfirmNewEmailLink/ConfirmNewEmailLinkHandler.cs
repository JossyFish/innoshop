using MediatR;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.ConfirmNewEmailLink
{
    public sealed class ConfirmNewEmailLinkHandler : IRequestHandler<ConfirmNewEmailLinkCommand, Unit>
    {
        ICacheUsersRepository _cache;
        IUsersRepository _usersRepository;
        ILinkTokenGenerator _linkTokenGenerator;

        public ConfirmNewEmailLinkHandler( ICacheUsersRepository cache, IUsersRepository usersRepository, ILinkTokenGenerator linkTokenGenerator)
        {
            _cache = cache;
            _usersRepository = usersRepository;
            _linkTokenGenerator = linkTokenGenerator;
        }
        public async Task<Unit> Handle(ConfirmNewEmailLinkCommand command, CancellationToken cancellationToken)
        {
            var userEmail = _linkTokenGenerator.ValidateToken(command.Token);
            if (string.IsNullOrEmpty(userEmail))
                throw new InvalidLinkTokenException("Invalid or expired token");

            var user = await _usersRepository.GetByEmailAsync(userEmail, cancellationToken);

            if(user == null)
                throw new UserNotFoundException(userEmail);

            var newEmailCache = await _cache.GetChangeEmailDataAsync(user.Id, cancellationToken);

            if (newEmailCache == null)
                throw new CacheDataNotFoundException("Confirm new Email", user.Email);

            if (newEmailCache.LinkToken != command.Token)
                throw new InvalidLinkTokenException("Token mismatch");


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
