using MediatR;

namespace UserService.Application.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<List<GetAllUsersResponse>>
    {
    }
}
