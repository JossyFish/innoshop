using ProductService.Domain.ValueObjects;

namespace ProductService.Domain.Entities
{
    public class ProductEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Price Price { get; set; } = new Price(0); 
        public int Quantity { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
