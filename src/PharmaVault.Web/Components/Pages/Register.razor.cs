using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;

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

            Navigation.NavigateTo("/");
        }
        catch (Exception)
        {
            ErrorMessage = "An error occurred during registration. The email might already be in use.";
        }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;
    }
}