using Microsoft.AspNetCore.Components;

namespace PharmaVault.Web.Components.Pages;

public partial class Dashboard : ComponentBase
{
    public string CurrentDate { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        CurrentDate = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
    }
}