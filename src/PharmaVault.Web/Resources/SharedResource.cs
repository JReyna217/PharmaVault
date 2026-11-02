using System.Globalization;
using System.Resources;

namespace PharmaVault.Web;

/// <summary>
/// Clase de recursos en el espacio de nombres raíz (PharmaVault.Web)
/// utilizada por IStringLocalizer&lt;SharedResource&gt; y por
/// System.ComponentModel.DataAnnotations (mediante ErrorMessageResourceType).
/// </summary>
public class SharedResource
{
    private static ResourceManager? _resourceManager;

    public static ResourceManager ResourceManager =>
        _resourceManager ??= new ResourceManager(
            "PharmaVault.Web.SharedResource",
            typeof(SharedResource).Assembly);

    public static string? GetString(string name, CultureInfo? culture = null) =>
        ResourceManager.GetString(name, culture ?? CultureInfo.CurrentUICulture);

    // Propiedades estáticas requeridas por DataAnnotations (ValidationAttribute reflection lookup)
    public static string? Validation_NameRequired => GetString(nameof(Validation_NameRequired));
    public static string? Validation_NameLength => GetString(nameof(Validation_NameLength));
    public static string? Validation_EmailRequired => GetString(nameof(Validation_EmailRequired));
    public static string? Validation_EmailInvalid => GetString(nameof(Validation_EmailInvalid));
    public static string? Validation_SubjectRequired => GetString(nameof(Validation_SubjectRequired));
    public static string? Validation_SubjectLength => GetString(nameof(Validation_SubjectLength));
    public static string? Validation_MessageRequired => GetString(nameof(Validation_MessageRequired));
    public static string? Validation_MessageLength => GetString(nameof(Validation_MessageLength));
    public static string? Validation_PasswordRequired => GetString(nameof(Validation_PasswordRequired));
    public static string? Validation_PasswordMinLength => GetString(nameof(Validation_PasswordMinLength));
}
