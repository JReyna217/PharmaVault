using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using PharmaVault.Core.Interfaces;

namespace PharmaVault.Web.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject] 
    public IAuthService AuthService { get; set; } = default!;

    [Inject] 
    public NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromForm(FormName = "LoginForm")]
    public LoginViewModel LoginModel { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task HandleLoginAsync()
    {
        ErrorMessage = null;

        var user = await AuthService.LoginAsync(LoginModel.Email, LoginModel.Password);

        if (user != null)
        {
            Navigation.NavigateTo("/dashboard");
        }
        else
        {
            ErrorMessage = "Invalid email or password.";
        }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}