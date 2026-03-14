using System;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;

namespace PharmaVault.Data.Persistence;

public class UserDao : IUserDao
{
    private readonly string _connectionString;

    // Inyectamos IConfiguration para leer los User Secrets o el appsettings.json
    public UserDao(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("La cadena de conexión no está configurada.");
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Usamos RETURNING user_id que es una característica excelente de PostgreSQL 
        // para obtener el ID recién creado en una sola vuelta al servidor.
        var query = @"
            INSERT INTO users (email, password_hash, full_name)
            VALUES (@email, @password_hash, @full_name)
            RETURNING user_id;";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("full_name", user.FullName);

        // ExecuteScalarAsync ejecuta el query y retorna la primera columna de la primera fila (nuestro user_id)
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT user_id, email, password_hash, full_name, registration_date 
            FROM users 
            WHERE email = @email;";

        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("email", email);

        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return new User
            {
                UserId = reader.GetInt32(0),
                Email = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FullName = reader.GetString(3),
                RegistrationDate = reader.GetDateTime(4)
            };
        }
        
        return null; // Retorna null si el correo no existe
    }
}
