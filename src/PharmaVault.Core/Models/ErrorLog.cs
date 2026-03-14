using System;

namespace PharmaVault.Core.Models;

public class ErrorLog
{
    public int LogId { get; set; }
    public string OriginLayer { get; set; } = string.Empty;
    public string MainObject { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime ErrorDate { get; set; }
    public int? UserId { get; set; }
    public Guid IncidentNumber { get; set; }
}
