namespace UserService.Application.Queries.GetSellers
{
    public record GetSellersResponse
    (
         List<SellerResponse> Sellers,
         int TotalCount,
         int PageNumber,
         int PageSize,
         int TotalPages
    );

    public record SellerResponse
    (
        Guid Id, 
        string Name
    );
}
