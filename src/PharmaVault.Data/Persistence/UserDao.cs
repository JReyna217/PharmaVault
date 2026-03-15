using System;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;
using PharmaVault.Data.Extensions;

namespace PharmaVault.Data.Persistence;

public class UserDao : IUserDao
{
    private readonly string _connectionString;

    public UserDao(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("La cadena de conexión no está configurada.");
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            INSERT INTO users (email, password_hash, full_name)
            VALUES (@email, @password_hash, @full_name)
            RETURNING user_id;";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("full_name", user.FullName);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT user_id AS UserId, 
            email AS Email, 
            password_hash AS PasswordHash, 
            full_name AS FullName, 
            registration_date AS RegistrationDate 
            FROM users 
            WHERE email = @email;";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("email", email);

        return await command.FillToObjectAsync<User>();
    }
}
