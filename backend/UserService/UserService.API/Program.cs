using Auth.Core.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserService.API.Exceptions;
using UserService.Application;
using UserService.Domain.Models;
using UserService.Infrastructure;
using UserService.Infrastructure.Data;

namespace UserService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddProblemDetails();

            //Custom authorization
            builder.Services.AddAuth();

            builder.Services.Configure<Auth.Core.Data.AuthorizationOptions>(
                builder.Configuration.GetSection("AuthorizationOptions"));
            builder.Services.AddSingleton(provider =>
                provider.GetRequiredService<IOptions<Auth.Core.Data.AuthorizationOptions>>().Value);

            //Add layers
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);


            builder.Services.AddHttpContextAccessor();

            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
            builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(nameof(EmailOptions)));
            builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection(nameof(CacheOptions)));
            builder.Services.Configure<LinkTokenOptions>(builder.Configuration.GetSection(nameof(LinkTokenOptions)));
            builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(nameof(RabbitMQOptions)));

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<UserDBContext>();

                    await context.Database.MigrateAsync();

                    var initializer = services.GetRequiredService<DbInitializer>();
                    await initializer.SeedAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while initializing the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(x =>
            {
                x.WithOrigins("http://localhost:3000")
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials();
            });

            app.UseHttpsRedirection();
            app.UseExceptionHandler();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
