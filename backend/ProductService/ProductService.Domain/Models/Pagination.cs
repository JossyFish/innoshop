namespace ProductService.Domain.Models
{
    public class Pagination
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
