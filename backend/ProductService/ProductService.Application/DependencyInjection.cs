using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductService.Application.Behaviors;
using ProductService.Application.Commands.AddProduct;
using ProductService.Application.Events;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;
using ProductService.Domain.Models;

namespace ProductService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AddProductCommand).Assembly);
            });

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserDeletedEventConsumer>();
                x.AddConsumer<UserDeactivatedEventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMQOptions = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;

                    cfg.Host(rabbitMQOptions.Host, "/", h =>
                    {
                        h.Username(rabbitMQOptions.Username);
                        h.Password(rabbitMQOptions.Password);
                    });

                    cfg.ReceiveEndpoint("user-deleted", e =>
                    {
                        e.ConfigureConsumer<UserDeletedEventConsumer>(context);
                        e.PrefetchCount = 10;
                    });

                    cfg.ReceiveEndpoint("user-deactivated", e =>
                    {
                        e.ConfigureConsumer<UserDeactivatedEventConsumer>(context);
                        e.PrefetchCount = 10;
                    });

                    cfg.UseMessageRetry(r => r.Interval(3, 1000));
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IFindProductService, FIndProductService>();
            services.AddValidatorsFromAssemblyContaining<AddProductValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
