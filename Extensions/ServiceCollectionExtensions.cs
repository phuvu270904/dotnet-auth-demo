using MyWebApi.Interfaces;
using MyWebApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MyWebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<JwtService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJobService, JobService>();

            return services;
        }
    }
}