using Benzeny.Domain.IRepository;
using Benzeny.Infra.Data;
using Benzeny.Infra.Repository;
using BenzenyMain.Application.Contracts.Export;
using BenzenyMain.Domain.IRepository;
using BenzenyMain.Infra.Export.Excel;
using BenzenyMain.Infra.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Benzeny.Infra
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
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICarRepository, CarRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAdsRepository, AdsRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<ICarTypeRepository, CarTypeRepository>();
            services.AddScoped<ICarModelRepository, CarModelRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IPlateTypeRepository, PlateTypeRepository>();
            services.AddScoped<ICarBrandRepository, CarBrandRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddTransient<IDriversTemplateBuilder, DriversTemplateExcelBuilder>();

            return services;
        }
    }
}