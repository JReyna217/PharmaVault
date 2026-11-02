using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.JSInterop;

namespace PharmaVault.Web.Services;

/// <summary>
/// Servicio Scoped que orquesta la información cultural en PharmaVault,
/// sincronizando los hilos del circuito de Blazor Server y gestionando la
/// compatibilidad con cookies de ASP.NET Core (.AspNetCore.Culture).
/// </summary>
public class CultureStateService
{
    private readonly IJSRuntime _jsRuntime;
    public const string DefaultCulture = "es-MX";
    public static readonly string[] SupportedCultures = ["es-MX", "es", "en-US", "en"];

    public event Action? OnCultureChanged;

    public CultureStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Obtiene la cultura actual basada en el hilo de interfaz de usuario activo.
    /// </summary>
    public string CurrentCulture => CultureInfo.CurrentUICulture.Name;

    /// <summary>
    /// Determina si la cultura activa actual es Español.
    /// </summary>
    public bool IsSpanish => string.IsNullOrEmpty(CurrentCulture) || CurrentCulture.StartsWith("es", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Cambia la cultura activa en tiempo de ejecución, actualiza los hilos predeterminados
    /// del proceso, guarda la cookie para peticiones SSR/HTTP futuras y notifica a los suscriptores.
    /// </summary>
    public async Task SetCultureAsync(string cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode) || !SupportedCultures.Contains(cultureCode))
        {
            cultureCode = DefaultCulture;
        }

        var cultureInfo = new CultureInfo(cultureCode);

        // 1. Sincronizar hilos actuales y predeterminados para el circuito activo
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        // 2. Persistir cookie estándar de ASP.NET Core (.AspNetCore.Culture) en el cliente
        try
        {
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureInfo));
            var jsCode = $"document.cookie = '{CookieRequestCultureProvider.DefaultCookieName}=' + encodeURIComponent('{cookieValue}') + '; path=/; max-age=31536000; SameSite=Lax';";
            await _jsRuntime.InvokeVoidAsync("eval", jsCode);
        }
        catch
        {
            // Falla silenciosa si JS aún no está interactivo (ej. durante prerendering)
        }

        // 3. Notificar a componentes y layouts suscritos para re-renderizado reactivo
        OnCultureChanged?.Invoke();
    }
}
