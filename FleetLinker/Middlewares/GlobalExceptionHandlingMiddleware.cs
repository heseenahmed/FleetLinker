using FleetLinker.Domain.Entity.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Net;
using System.Text.Json;

namespace FleetLinker.API.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed.");
            await WriteErrorResponseAsync(context, HttpStatusCode.BadRequest, "Validation Error", ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList());
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Key not found.");
            await WriteErrorResponseAsync(context, HttpStatusCode.NotFound, "Not Found", new List<string> { ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument.");
            await WriteErrorResponseAsync(context, HttpStatusCode.BadRequest, "Invalid Argument", new List<string> { ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized.");
            await WriteErrorResponseAsync(context, HttpStatusCode.Unauthorized, "Unauthorized", new List<string> { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation.");
            await WriteErrorResponseAsync(context, HttpStatusCode.BadRequest, "Operation Failed", new List<string> { ex.Message });
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning(ex, "Invalid operation.");
            await WriteErrorResponseAsync(context, HttpStatusCode.BadRequest, "Operation Failed", new List<string> { ex.Message });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error.");
            await WriteErrorResponseAsync(context, HttpStatusCode.Conflict, "Database Error", new List<string>
            {
                "A database error occurred.",
                ex.InnerException?.Message ?? ex.Message
            });
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error occurred.");
            await WriteErrorResponseAsync(context, HttpStatusCode.InternalServerError, "SQL Error", new List<string> { ex.Message });
        }
        catch (ApplicationException appEx)
        {
            _logger.LogError(appEx, "Application-level exception.");
            var errors = new List<string>();

            if (!string.IsNullOrEmpty(appEx.Message))
                errors.Add(appEx.Message);

            if (appEx.InnerException != null && !string.IsNullOrEmpty(appEx.InnerException.Message))
                errors.Add(appEx.InnerException.Message);

            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.Conflict,
                "Application Error",
                errors
            );
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            await WriteErrorResponseAsync(context, HttpStatusCode.InternalServerError, "Server Error", new List<string>
            {
                "An unexpected error occurred.",
                ex.Message
            });
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, string msg, List<string> errors)
    {
        if (context.Response.HasStarted)
            return;

        var response = new APIResponse<string>
        {
            ApiStatusCode = (int)statusCode,
            Result = "Failed",
            Msg = msg,
            Errors = errors
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await context.Response.WriteAsync(json);
    }
}
