using Benzeny.API;
using Benzeny.API.Filter;
using Benzeny.API.Middlewares;
using Benzeny.Application;
using Benzeny.Application.Common;
using Benzeny.Domain.Entity;
using Benzeny.Infra;
using BenzenyMain.Infra.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using QuestPDF.Infrastructure;
using SendEmailsWithDotNet5.Settings;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add<ValidateModelFilter>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

//pdf configuration
QuestPDF.Settings.License = LicenseType.Community;

//IDistributedCache
builder.Services.AddDistributedMemoryCache();
//global exception handler 
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
// Add services to the container.
builder.Services.AddAppServices();
builder.Services.AddInfraServices(builder.Configuration);
builder.Services.AddAPIServices(builder.Configuration);
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<JwtSettings>>().Value);

builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//Background Service

// Program.cs
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true);
builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


// CORS 
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

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();


app.UseStaticFiles();

app.Run();