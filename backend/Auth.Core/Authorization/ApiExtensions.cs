using Auth.Core.Data;
using Auth.Core.Services;
using Auth.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthorizationOptions = Auth.Core.Data.AuthorizationOptions;

namespace Auth.Core.Authorization
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: false)
                .Build();

            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            services.Configure<AuthorizationOptions>(configuration.GetSection(nameof(Auth.Core.Data.AuthorizationOptions)));

            services.AddSingleton(provider =>
                provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value);

            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["myCookie"];
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            return services;
        }

        //public static void AddApiAuthentication(this IServiceCollection services, IOptions<JwtOptions> jwtOptions)
        //{
        //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        //        {
        //            options.TokenValidationParameters = new()
        //            {
        //                ValidateIssuer = false,
        //                ValidateAudience = false,
        //                ValidateLifetime = true,
        //                ValidateIssuerSigningKey = true,
        //                IssuerSigningKey = new SymmetricSecurityKey(
        //                    Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey))
        //            };
        //            options.Events = new JwtBearerEvents
        //            {
        //                OnMessageReceived = context =>
        //                {
        //                    context.Token = context.Request.Cookies["myCookie"];
        //                    return Task.CompletedTask;
        //                }
        //            };
        //        });
        //}
    }
}