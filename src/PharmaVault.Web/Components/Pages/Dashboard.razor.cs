using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;

namespace PharmaVault.Web.Components.Pages;

public partial class Dashboard : ComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState>? AuthState { get; set; }

    [Inject]
    public IInventoryDao InventoryDao { get; set; } = default!;

    [Inject]
    public AuthenticationStateProvider AuthProvider { get; set; } = default!;
    
    public string UserName { get; set; } = string.Empty;
    public string CurrentDate { get; set; } = string.Empty;

    protected DashboardStatsDto? _stats;
    protected int _userId;

    protected override async Task OnInitializedAsync()
    {
        if (AuthState != null)
        {
            var authState = await AuthState;
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                UserName = user.Identity.Name ?? "User";
            }

            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
            if (int.TryParse(userIdString, out int parsedId))
            {
                _userId = parsedId;
                _stats = await InventoryDao.GetDashboardStatsAsync(_userId);
            }
        }

        CurrentDate = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
    }
}