using FleetLinker.Application.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace FleetLinker.Application
{
    public static class AppConfigureServices
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            return services;
        }
    }
}