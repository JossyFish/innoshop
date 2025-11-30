using MediatR;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Queries.GetAllUsers
{
    public sealed class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<GetAllUsersResponse>>
    {
        private readonly IUsersRepository _usersRepository;

        public GetAllUsersHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<List<GetAllUsersResponse>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var users = await _usersRepository.GetAllAsync(cancellationToken);
            
            return users.Select(user => new GetAllUsersResponse(
                        user.Id,
                        user.Name,
                        user.Email,
                        user.IsActive,
                        user.CreatedAt,
                        user.Roles
                    )).ToList();
        }
    }
}
