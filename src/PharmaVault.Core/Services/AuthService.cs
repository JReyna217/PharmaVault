using System;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;

namespace PharmaVault.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserDao _userDao;

    public AuthService(IUserDao userDao)
    {
        _userDao = userDao;
    }

    public async Task<int> RegisterAsync(User user, string plainPassword)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        
        return await _userDao.CreateAsync(user);
    }

    public async Task<User?> LoginAsync(string email, string plainPassword)
    {
        var user = await _userDao.GetByEmailAsync(email);
        
        if (user == null)
            return null;

        bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, user.PasswordHash);
        
        if (!isValid)
            return null;

        return user;
    }
}
