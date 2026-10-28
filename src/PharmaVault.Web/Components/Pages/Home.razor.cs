using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;

namespace PharmaVault.Web.Components.Pages;

public partial class Home : ComponentBase
{
    protected ContactFormModel ContactModel { get; set; } = new();
    protected bool IsSubmitting { get; set; } = false;
    protected bool ShowSuccessAlert { get; set; } = false;

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

    public class ContactFormModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 80 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un formato de correo electrónico válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "El asunto debe tener entre 3 y 120 caracteres.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje es obligatorio.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "El mensaje debe contener al menos 10 caracteres.")]
        public string Message { get; set; } = string.Empty;
    }
}
