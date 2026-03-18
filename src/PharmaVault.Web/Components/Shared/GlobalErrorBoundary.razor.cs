using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using PharmaVault.Core.Exceptions;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;

namespace PharmaVault.Web.Components.Shared;

public partial class GlobalErrorBoundary : ErrorBoundary
{
    [Inject]
    private IErrorLogDao ErrorLogDao { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthProvider { get; set; } = default!;

    [Inject]
    private ILogger<GlobalErrorBoundary> Logger { get; set; } = default!;

    private Guid _incidentId = Guid.Empty;

    protected override Task OnErrorAsync(Exception exception)
    {
        _ = ProcessErrorInBackgroundAsync(exception);
        
        return Task.CompletedTask;
    }

    private async Task ProcessErrorInBackgroundAsync(Exception exception)
    {
        try
        {
            int? userId = null;
            try 
            {
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                var userIdString = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdString, out int parsedId)) userId = parsedId;
            }
            catch { /* Ignore */ }

            string safeMessage = exception.Message ?? "Unknown Error";
            if (safeMessage.Length > 3900) safeMessage = safeMessage.Substring(0, 3900);

            string safeDescription = exception.StackTrace ?? string.Empty;
            if (safeDescription.Length > 3900) safeDescription = safeDescription.Substring(0, 3900) + "\n...[TRUNCATED]";

            if (exception is ErrorExceptionResponse controlledEx)
            {
                _incidentId = await ErrorLogDao.LogErrorAsync(controlledEx.Details, userId);
                Logger.LogWarning("Controlled exception caught: {Message}. Incident: {IncidentNumber}", controlledEx.Message, _incidentId);
            }
            else
            {
                var genericRequest = new ExceptionLogDto
                {
                    OriginLayer = "Frontend", 
                    MainObject = exception.TargetSite?.DeclaringType?.Name ?? "UnknownComponent",
                    MethodName = exception.TargetSite?.Name ?? "UnknownMethod",
                    ErrorMessage = safeMessage,
                    Description = safeDescription
                };

                _incidentId = await ErrorLogDao.LogErrorAsync(genericRequest, userId);
                Logger.LogError(exception, "Unhandled UI exception. Incident: {IncidentNumber}", _incidentId);
            }
        }
        catch (Exception dbLogEx)
        {
            _incidentId = Guid.NewGuid();
            Logger.LogError(dbLogEx, "CRITICAL FALLBACK: Could not save error to database! Incident displayed: {IncidentNumber}", _incidentId);
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    public new void Recover()
    {
        _incidentId = Guid.Empty; 
        base.Recover();
    }
}