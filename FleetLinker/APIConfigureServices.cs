using FleetLinker.API.Filter;
using FleetLinker.API.Localization;
using FleetLinker.API.Middlewares;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Domain.Entity;
using FleetLinker.Infra.Behaviors;
using FleetLinker.Infra.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
namespace FleetLinker.API
{
    public static class APIConfigureServices
    {
        public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<GlobalExceptionHandlingMiddleware>();
            //services.AddControllers(options =>
            //{
            //    options.Filters.Add<ValidateUserStateFilter>();
            //});
            services.AddControllers()
                    .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true);
            // Add Localization Services
            services.AddLocalization(options => options.ResourcesPath = "");
            // Configure supported cultures (Arabic as default)
            var supportedCultures = new[]
            {
                new CultureInfo("ar"),
                new CultureInfo("en")
            };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("ar"); // Arabic is default
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.ApplyCurrentCultureToResponseHeaders = true;
            });
            services.AddControllers(options => options.Filters.Add<ValidateModelFilter>())
                    .AddDataAnnotationsLocalization()
                    .AddViewLocalization()
                    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });

            services.AddEndpointsApiExplorer();
            
            services.AddScoped<IAppLocalizer, AppLocalizer>();
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
           {
               options.Password.RequireDigit = false;
               options.Password.RequiredLength = 6;
               options.Password.RequireNonAlphanumeric = false;
               options.Password.RequireUppercase = false;
               options.Password.RequireLowercase = false;
               options.Password.RequiredUniqueChars = 0;
               options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
               options.Lockout.MaxFailedAccessAttempts = 5;
               options.Lockout.AllowedForNewUsers = true;
               options.SignIn.RequireConfirmedEmail = false;
               options.SignIn.RequireConfirmedPhoneNumber = false;
               options.User.RequireUniqueEmail = false;
           })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret!))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        var userManager = ctx.HttpContext.RequestServices
                            .GetRequiredService<UserManager<ApplicationUser>>();
                        var userId = ctx.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? ctx.Principal!.FindFirstValue(JwtRegisteredClaimNames.Sub);
                        var user = await userManager.FindByIdAsync(userId);
                        var currentStamp = await userManager.GetSecurityStampAsync(user);
                        var tokenStamp = ctx.Principal!.FindFirst("sstamp")?.Value;
                        if (tokenStamp != currentStamp)
                        {
                            ctx.Fail("Security stamp invalid (user logged out or password changed).");
                        }
                    }
                };
            });
            services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc("v1", new OpenApiInfo
               {
                   Title = "FleetLinker API",
                   Version = "v1"
               });
               c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
               {
                   Name = "Authorization",
                   Type = SecuritySchemeType.Http,
                   Scheme = "Bearer",
                   BearerFormat = "JWT",
                   In = ParameterLocation.Header,
                   Description = "Enter your JWT token only , do not enter Bearer word Like This Format: **{your token}**"
               });
               c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
           });
            return services;
        }
    }
}
