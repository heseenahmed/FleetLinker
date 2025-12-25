using FleetLinker.API;
using FleetLinker.API.Middlewares;
using FleetLinker.Application;
using FleetLinker.Domain.Entity;
using FleetLinker.Infra;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Layer registrations
builder.Services.AddAppServices();
builder.Services.AddInfraServices(builder.Configuration);
builder.Services.AddAPIServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetLinker API V1");
        c.DisplayRequestDuration();
    });
}

FileHelperStartup.Configure(app); 

// Middleware (Exception first)
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Static files
app.UseStaticFiles();

// Localization (MUST be before auth)
app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

// CORS
app.UseCors("MyPolicy");

// Security & pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
