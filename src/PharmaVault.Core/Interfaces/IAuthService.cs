using System;
using PharmaVault.Core.Models;

namespace PharmaVault.Core.Interfaces;

public interface IAuthService
{
    Task<int> RegisterAsync(User user, string plainPassword);
    
    Task<User?> LoginAsync(string email, string plainPassword);
}
