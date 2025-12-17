using FleetLinker.API;
using FleetLinker.API.Middlewares;
using FleetLinker.Application;
using FleetLinker.Domain.Entity;
using FleetLinker.Infra;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// ?? Layer registrations
builder.Services.AddAppServices();
builder.Services.AddInfraServices(builder.Configuration);
builder.Services.AddAPIServices(builder.Configuration);

// ?? JWT settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<JwtSettings>>().Value);

// ?? Middleware
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

FileHelperStartup.Configure(app); 

// ?? Swagger
app.UseSwagger();
app.UseSwaggerUI();

// ?? Static files
app.UseStaticFiles();

// ?? Localization (MUST be before auth)
app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

// ?? CORS
app.UseCors("MyPolicy");

// ?? Security & pipeline
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();
app.Run();
