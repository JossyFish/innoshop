using Auth.Core.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductService.API.Exceptions;
using ProductService.Application;
using ProductService.Domain.Models;
using ProductService.Infrastructure;
using ProductService.Infrastructure.Data;

namespace ProductService
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

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddRouting();

            builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(nameof(RabbitMQOptions)));

            builder.Services.AddApplication();
            builder.Services.AddAuth();

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ProductDBContext>();

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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
