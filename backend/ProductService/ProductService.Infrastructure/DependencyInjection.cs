using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProductDBContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(nameof(ProductDBContext)));
            });

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<DbInitializer>();

            return services;
        }
    }
}
