using MediatR;

namespace UserService.Application.Queries.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<GetCurrentUserResponse>
    {
    }
}
