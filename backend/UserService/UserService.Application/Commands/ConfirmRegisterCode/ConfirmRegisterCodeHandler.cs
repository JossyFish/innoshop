using Auth.Core.Services.Interfaces;
using MediatR;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;


namespace UserService.Application.Commands.ConfirmRegisterCode
{
    public sealed class ConfirmRegisterCodeHandler : IRequestHandler<ConfirmRegisterCodeCommand, string>
    {
        ICacheUsersRepository _cache;
        IUsersRepository _usersRepository;
        IJwtProvider _jwtProvider;
        
        public ConfirmRegisterCodeHandler(ICacheUsersRepository cache, IUsersRepository usersRepository, IJwtProvider jwtProvider)
        {
            _cache = cache;
            _usersRepository = usersRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<string> Handle(ConfirmRegisterCodeCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await _usersRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (existingUser != null)
                throw new UserAlreadyExistException(command.Email);

            var userCache = await _cache.GetRegistrationDataByEmailAsync(command.Email, cancellationToken);

            if (userCache == null)
                throw new CacheDataNotFoundException("Register", command.Email);

           if(userCache.ConfirmationCode != command.ConfirmationCode)
                throw new InvalidConfirmCodeException(command.Email);

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

            var token = _jwtProvider.GenerateToken(user.Id, Auth.Core.Enums.Role.User);

            return token;
        }
    }
}
