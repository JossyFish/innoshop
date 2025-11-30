using Auth.Core.Services.Interfaces;
using MediatR;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Commands.ConfirmRegisterLink
{
    public sealed class ConfirmRegisterLinkHandler : IRequestHandler<ConfirmRegisterLinkCommand, string>
    {
        ICacheUsersRepository _cache;
        IUsersRepository _usersRepository;
        IJwtProvider _jwtProvider;
        ILinkTokenGenerator _linkTokenGenerator;

        public ConfirmRegisterLinkHandler( ICacheUsersRepository cache, IUsersRepository usersRepository, IJwtProvider jwtProvider,
                                            ILinkTokenGenerator linkTokenGenerator)
        {
            _cache = cache;
            _usersRepository = usersRepository;
            _jwtProvider = jwtProvider;
            _linkTokenGenerator = linkTokenGenerator;
        }

        public async Task<string> Handle(ConfirmRegisterLinkCommand command, CancellationToken cancellationToken)
        {
            var email = _linkTokenGenerator.ValidateToken(command.Token);
            if (string.IsNullOrEmpty(email))
                throw new InvalidLinkTokenException("Invalid or expired token");

            var existingUser = await _usersRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser != null)
                throw new UserAlreadyExistException(email);

            var userCache = await _cache.GetRegistrationDataByEmailAsync(email, cancellationToken);
            if (userCache == null)
                throw new CacheDataNotFoundException("Register", email);

            if (userCache.LinkToken != command.Token)
                throw new InvalidLinkTokenException("Token mismatch");

            var user = User.Create(
                userCache.Id,
                userCache.Name,
                userCache.Email,
                userCache.PasswordHash,
                true,
                DateTime.UtcNow
            );

            await _usersRepository.AddAsync(user, cancellationToken);
            await _cache.RemoveRegistrationDataAsync(user.Id, cancellationToken);

            return _jwtProvider.GenerateToken(user.Id, Auth.Core.Enums.Role.User);
        }
    }
}
