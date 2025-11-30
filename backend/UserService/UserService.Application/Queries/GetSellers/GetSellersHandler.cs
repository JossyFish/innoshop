using MediatR;
using UserService.Domain.Models;
using UserService.Infrastructure.Repositories;

namespace UserService.Application.Queries.GetSellers
{
    public class GetSellersHandler : IRequestHandler<GetSellersQuery, GetSellersResponse>
    {
        private readonly IUsersRepository _usersRepository;

        public GetSellersHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<GetSellersResponse> Handle(GetSellersQuery query, CancellationToken cancellationToken)
        {
            var pageNumber = Math.Max(1, query.PageNumber);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);

            var filters = new UserFilters
            {
                Search = query.Search,
                IsActive = true, 
                RoleName = "User" 
            };

            var pagination = new Pagination
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var (users, totalCount) = await _usersRepository.GetFilterUsersAsync(
                filters,
                pagination,
                cancellationToken
            );

            var sellers = users.Select(user => new SellerResponse(
                user.Id,
                user.Name
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new GetSellersResponse(
                sellers,
                totalCount,
                pageNumber,
                pageSize,
                totalPages
            );
        }
    }
}
