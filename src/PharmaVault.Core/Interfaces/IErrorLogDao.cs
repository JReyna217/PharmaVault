using PharmaVault.Core.Models;

namespace PharmaVault.Core.Interfaces;

public interface IErrorLogDao
{
    Task<Guid> LogErrorAsync(ExceptionLogDto request, int? userId = null);
}