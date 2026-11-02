using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using PharmaVault.Web;

namespace PharmaVault.Web.Components.Pages;

public partial class Home : ComponentBase, IDisposable
{
    protected ContactFormModel ContactModel { get; set; } = new();
    protected bool IsSubmitting { get; set; } = false;
    protected bool ShowSuccessAlert { get; set; } = false;

    protected override void OnInitialized()
    {
        CultureState.OnCultureChanged += HandleCultureChanged;
    }

    private void HandleCultureChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    protected async Task HandleContactSubmitAsync()
    {
        IsSubmitting = true;
        ShowSuccessAlert = false;

        // Simulación de procesamiento asíncrono
        await Task.Delay(600);

        ShowSuccessAlert = true;
        ContactModel = new ContactFormModel();
        IsSubmitting = false;
    }

    public void Dispose()
    {
        CultureState.OnCultureChanged -= HandleCultureChanged;
    }

    public class ContactFormModel
    {
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_NameRequired")]
        [StringLength(80, MinimumLength = 2, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_NameLength")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_EmailInvalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_SubjectRequired")]
        [StringLength(120, MinimumLength = 3, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_SubjectLength")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_MessageRequired")]
        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_MessageLength")]
        public string Message { get; set; } = string.Empty;
    }
}
