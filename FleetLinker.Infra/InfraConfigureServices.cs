using FleetLinker.Domain.IRepository;
using FleetLinker.Infra.Data;
using FleetLinker.Infra.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace FleetLinker.Infra
{
    public static class InfraConfigureServices
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("AppDBContext"));
                Console.WriteLine($"Connection String: {options.UseSqlServer(configuration.GetConnectionString("AppDBContext"))}");
            });
             services.AddScoped<IEmailSender, MailSender>();  // Register the MailSender service
            services.AddTransient<IMailRepository, MailRepository>();
            services.AddScoped<IServiceProvider, ServiceProvider>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}