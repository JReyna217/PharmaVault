using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace PharmaVault.Web.Components.Pages;

public partial class Dashboard : ComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState>? AuthState { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    public string CurrentDate { get; set; } = string.Empty;

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
        }

        CurrentDate = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
    }
}