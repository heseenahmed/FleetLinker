using FleetLinker.API.Middlewares;
using FleetLinker.Domain.Entity;
using FleetLinker.Infra.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace FleetLinker.API
{
    public static class APIConfigureServices
    {
        public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<GlobalExceptionHandlingMiddleware>();
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
                   Title = "Benzeny API",
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
