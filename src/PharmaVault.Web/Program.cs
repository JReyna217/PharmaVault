using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Services;
using PharmaVault.Data.Persistence;
using PharmaVault.Web.Components;
using PharmaVault.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//DAOs
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<IMedicineCatalogDao, MedicineCatalogDao>();
builder.Services.AddScoped<IInventoryDao, InventoryDao>();
builder.Services.AddScoped<IErrorLogDao, ErrorLogDao>();

//Services
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/"; // If someone try to enter without cookie, go to login
        options.Cookie.Name = "PharmaVaultSession";
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Session time
    });

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoint to destroy cookie and redirect to login
app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.Run();
