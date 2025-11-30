namespace UserService.Domain.Models
{
    public class UserFilters
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public string? RoleName { get; set; }
    }
}
