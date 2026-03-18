using System.Net;
using System.Security.Claims;
using System.Text.Json;
using PharmaVault.Core.Exceptions;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;

namespace PharmaVault.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IErrorLogDao errorLogDao)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, errorLogDao);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, IErrorLogDao errorLogDao)
    {
        // 1. Try to get the ID from user
        int? userId = null;
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int parsedId))
        {
            userId = parsedId;
        }

        Guid incidentNumber;
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        string message;

        // 2. Clasify the exception
        if (exception is ErrorExceptionResponse controlledEx)
        {
            // Known exception
            statusCode = controlledEx.StatusCode;
            message = controlledEx.Message;
            incidentNumber = await errorLogDao.LogErrorAsync(controlledEx.Details, userId);
            
            _logger.LogWarning("Controlled exception {ErrorCode}: {Message}. Incident: {IncidentNumber}", 
                controlledEx.ErrorCode, controlledEx.Message, incidentNumber);
        }
        else
        {
            // Unknown exception
            message = "An unexpected internal server error occurred.";
            var genericRequest = new ExceptionLogDto
            {
                OriginLayer = "Backend",
                MainObject = exception.TargetSite?.DeclaringType?.Name ?? "Unknown",
                MethodName = exception.TargetSite?.Name ?? "Unknown",
                ErrorMessage = exception.Message,
                Description = exception.StackTrace
            };

            incidentNumber = await errorLogDao.LogErrorAsync(genericRequest, userId);
            
            _logger.LogError(exception, "Unhandled exception. Incident: {IncidentNumber}", incidentNumber);
        }

        // 3. Response format
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new 
        { 
            error = message, 
            incidentId = incidentNumber,
            status = statusCode
        });

        await context.Response.WriteAsync(result);
    }
}