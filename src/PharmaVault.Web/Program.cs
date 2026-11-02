using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Services;
using PharmaVault.Data.Persistence;
using PharmaVault.Web;
using PharmaVault.Web.Components;
using PharmaVault.Web.Middleware;
using PharmaVault.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Localization services
builder.Services.AddLocalization();
builder.Services.AddScoped<CultureStateService>();

// DAOs
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<IMedicineCatalogDao, MedicineCatalogDao>();
builder.Services.AddScoped<IInventoryDao, InventoryDao>();
builder.Services.AddScoped<IErrorLogDao, ErrorLogDao>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/login"; // If someone tries to enter without cookie, go to login
        options.Cookie.Name = "PharmaVaultSession";
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Session time
    });

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// Request Localization Middleware
var supportedCultures = CultureStateService.SupportedCultures;
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(CultureStateService.DefaultCulture)
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoint to toggle language for static SSR / direct HTTP requests
app.MapGet("/culture/set", (string culture, string? redirectUri, HttpContext context) =>
{
    if (!string.IsNullOrEmpty(culture) && CultureStateService.SupportedCultures.Contains(culture))
    {
        context.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                SameSite = SameSiteMode.Lax,
                Path = "/",
                IsEssential = true
            }
        );
    }

    if (string.IsNullOrEmpty(redirectUri) || !redirectUri.StartsWith("/"))
    {
        redirectUri = "/" + (redirectUri ?? "").TrimStart('/');
    }
    return Results.LocalRedirect(redirectUri);
});

// Endpoint to destroy cookie and redirect to login
app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});


app.Run();
