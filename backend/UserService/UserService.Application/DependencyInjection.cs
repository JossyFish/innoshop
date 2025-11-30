using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserService.Application.Behaviors;
using UserService.Application.Commands.CreateUser;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Domain.Models;

namespace UserService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
            });

            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMQOptions = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;

                    cfg.Host(rabbitMQOptions.Host, "/", h =>
                    {
                        h.Username(rabbitMQOptions.Username);
                        h.Password(rabbitMQOptions.Password);
                    });

                    cfg.UseMessageRetry(r => r.Interval(3, 1000));
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<ILinkTokenGenerator, LinkTokenGenerator>();   
            services.AddScoped<IBaseUserService, BaseUserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IConfirmCodeGenerator, ConfirmCodeGenerator>();

            return services;
        }
    }
}
