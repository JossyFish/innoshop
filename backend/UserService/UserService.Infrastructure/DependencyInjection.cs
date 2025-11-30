using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Templates;

namespace UserService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDBContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(nameof(UserDBContext)));
            });

            //if redis do not using, default using - memory cache
            ConfigureCache(services, configuration);

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ICacheUsersRepository, CacheUsersRepository>();
            services.AddScoped<IEmailTemplates, EmailTemplates>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<DbInitializer>();

            return services;
        }

        private static void ConfigureCache(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHybridCache();

            var redisConnection = configuration.GetConnectionString("Redis");
            var redisAvailable = false;

            if (!string.IsNullOrEmpty(redisConnection))
            {
                try
                {
                    var connection = ConnectionMultiplexer.Connect(redisConnection + ",connectTimeout=1000,abortConnect=false");
                    redisAvailable = connection.IsConnected;
                    connection.Close();
                }
                catch
                {
                    redisAvailable = false;
                }
            }

            if (redisAvailable)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                });
                Console.WriteLine("Redis cache configured");
            }
            else
            {
                services.AddDistributedMemoryCache();
                Console.WriteLine("Using in-memory cache");
            }
        }
    }
}

