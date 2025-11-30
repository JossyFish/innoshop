using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;
using ProductService.Domain.ValueObjects;

namespace ProductService.Infrastructure.Data
{
    public class DbInitializer
    {
        private readonly ProductDBContext _context;

        public DbInitializer(ProductDBContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var anyProductsExist = await _context.Products.AnyAsync();

            if (!anyProductsExist)
            {
                var exampleUserId = Guid.Parse("b9ed240f-be97-47a9-b00f-b99a96320e28");

                var products = new List<ProductEntity>
                {
                    new ProductEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "iPhone 15 Pro",
                        Description = "Latest Apple smartphone with advanced camera and A17 Pro chip",
                        Price = new Price(999.99m, Currency.BYN),
                        Quantity = 10,
                        IsActive = true,
                        UserId = exampleUserId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ProductEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Samsung S24",
                        Description = "Android flagship with AI features and excellent display",
                        Price = new Price(849.99m, Currency.BYN),
                        Quantity = 15,
                        IsActive = true,
                        UserId = exampleUserId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ProductEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "MacBook Air M3",
                        Description = "Lightweight laptop with Apple M3 chip for productivity",
                        Price = new Price(1299.99m, Currency.BYN),
                        Quantity = 5,
                        IsActive = true,
                        UserId = exampleUserId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ProductEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sony XM5",
                        Description = "Wireless noise-canceling headphones with premium sound quality",
                        Price = new Price(399.99m, Currency.BYN),
                        Quantity = 20,
                        IsActive = true,
                        UserId = exampleUserId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new ProductEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "iPad Pro 12.9",
                        Description = "Professional tablet with M2 chip and Liquid Retina XDR display",
                        Price = new Price(1099.99m, Currency.BYN),
                        Quantity = 8,
                        IsActive = true,
                        UserId = exampleUserId,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }
        }
    }
}