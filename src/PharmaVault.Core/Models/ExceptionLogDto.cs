using System;

namespace PharmaVault.Core.Models;

public class ExceptionLogDto
{
    public string OriginLayer { get; set; } = string.Empty;
    public string MainObject { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? Description { get; set; }
}
