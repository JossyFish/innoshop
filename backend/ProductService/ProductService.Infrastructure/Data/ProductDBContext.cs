using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;


namespace ProductService.Infrastructure.Data
{
    public class ProductDBContext(DbContextOptions<ProductDBContext> options) : DbContext(options)
    {
        public DbSet<ProductEntity> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDBContext).Assembly);
        }
    }
}
