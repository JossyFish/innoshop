using Auth.Core.Services.Interfaces;
using UserService.Application.Interfaces;
using UserService.Domain.Exceptions;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Services
{
    public class BaseUserService : IBaseUserService
    {
        IUserContext _userContext;
        IUsersRepository _usersRepository;

        public BaseUserService(IUserContext userContext, IUsersRepository usersRepository)
        {
            _userContext = userContext;
            _usersRepository = usersRepository;
        }

        public async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();
            var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);
            return user ?? throw new UserNotFoundByIdException(userId);
        }
    }
}
