using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;
using ApexCharts;

namespace PharmaVault.Web.Components.Pages;

public class InventoryChartData
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

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

    protected List<InventoryChartData> _chartData = new();
    protected ApexChartOptions<InventoryChartData> _chartOptions = new();

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

                if (_stats != null && _stats.TotalStock > 0)
                {
                    _chartData = new List<InventoryChartData>
                    {
                        new() { Status = "Good Condition", Count = _stats.GoodStock },
                        new() { Status = "Expiring Soon", Count = _stats.ExpiringSoonStock },
                        new() { Status = "Expired", Count = _stats.ExpiredStock }
                    };

                    _chartOptions = new ApexChartOptions<InventoryChartData>
                    {
                        Colors = new List<string> { "#198754", "#ffc107", "#dc3545" },
                        Chart = new Chart { Background = "transparent" }
                    };
                }
            }
        }

        CurrentDate = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
    }
}