using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;
using PharmaVault.Web;

namespace PharmaVault.Web.Components.Pages;

public partial class Register : ComponentBase
{
    [Inject]
    public IAuthService AuthService { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromForm(FormName = "RegisterForm")]
    public RegisterViewModel RegisterModel { get; set; } = default!;
    public string? ErrorMessage { get; set; }

    protected override void OnInitialized()
    {
        RegisterModel ??= new RegisterViewModel();
    }

    public async Task HandleRegisterAsync()
    {
        ErrorMessage = null;

        try
        {
            var newUser = new User
            {
                FullName = RegisterModel.FullName,
                Email = RegisterModel.Email
            };

            await AuthService.RegisterAsync(newUser, RegisterModel.Password);

            Navigation.NavigateTo("/login");
        }
        catch (Exception)
        {
            ErrorMessage = "An error occurred during registration. The email might already be in use.";
        }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_NameRequired")]
        [StringLength(80, MinimumLength = 2, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_NameLength")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_EmailInvalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_PasswordRequired")]
        [MinLength(6, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "Validation_PasswordMinLength")]
        public string Password { get; set; } = string.Empty;
    }
}
