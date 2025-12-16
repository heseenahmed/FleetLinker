using FleetLinker.API;
using FleetLinker.API.Filter;
using FleetLinker.API.Middlewares;
using FleetLinker.Application;
using FleetLinker.Application.Common;
using FleetLinker.Domain.Entity;
using FleetLinker.Infra;
using FleetLinker.Infra.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add Localization Services
builder.Services.AddLocalization(options => options.ResourcesPath = "");

// Configure supported cultures (Arabic as default)
var supportedCultures = new[]
{
    new CultureInfo("ar"),
    new CultureInfo("en")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ar"); // Arabic is default
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.ApplyCurrentCultureToResponseHeaders = true;
});

builder.Services.AddControllers(options => options.Filters.Add<ValidateModelFilter>())
    .AddDataAnnotationsLocalization()
    .AddViewLocalization()
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
builder.Services.AddAppServices();
builder.Services.AddInfraServices(builder.Configuration);
builder.Services.AddAPIServices(builder.Configuration);
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<JwtSettings>>().Value);
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true);
builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});
builder.Services.AddEndpointsApiExplorer(); 

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
}

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

IFileHelper.Configure(app.Services.GetRequiredService<IWebHostEnvironment>(),
    app.Services.GetRequiredService<IHttpContextAccessor>());

app.UseCors("MyPolicy");

// Enable Request Localization (MUST be before UseAuthorization)
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.MapControllers();
app.UseStaticFiles();
app.Run();