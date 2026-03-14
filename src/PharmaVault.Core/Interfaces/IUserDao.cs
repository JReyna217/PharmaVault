using System;
using PharmaVault.Core.Models;

namespace PharmaVault.Core.Interfaces;

public interface IUserDao
{
    Task<User?> GetByEmailAsync(string email);
    
    Task<int> CreateAsync(User user);
}
