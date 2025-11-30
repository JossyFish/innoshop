using MediatR;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Queries.GetCurrentUser
{
    public sealed class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
    {
        IBaseUserService _baseUserHandler;
        IUsersRepository _usersRepository;

        public GetCurrentUserHandler(IBaseUserService baseUserHandler, IUsersRepository usersRepository)
        {
            _baseUserHandler = baseUserHandler;
            _usersRepository = usersRepository;
        }

        public async Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
        {
            var user = await _baseUserHandler.GetCurrentUserAsync(cancellationToken);

            return new GetCurrentUserResponse(
                user.Id,
                user.Name,
                user.Email,
                user.IsActive,
                user.CreatedAt,
                user.Roles
                );
        }
    }
}
