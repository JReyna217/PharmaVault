using Microsoft.Extensions.Configuration;
using Npgsql;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;

namespace PharmaVault.Data.Persistence;

public class ErrorLogDao : IErrorLogDao
{
    private readonly string _connectionString;

    public ErrorLogDao(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<Guid> LogErrorAsync(ExceptionLogDto request, int? userId = null)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO error_logs (origin_layer, main_object, method_name, description, error_message, user_id)
            VALUES (@OriginLayer, @MainObject, @MethodName, @Description, @ErrorMessage, @UserId)
            RETURNING incident_number;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("OriginLayer", request.OriginLayer);
        cmd.Parameters.AddWithValue("MainObject", request.MainObject);
        cmd.Parameters.AddWithValue("MethodName", request.MethodName);
        cmd.Parameters.AddWithValue("Description", request.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("ErrorMessage", request.ErrorMessage);
        cmd.Parameters.AddWithValue("UserId", userId ?? (object)DBNull.Value);

        var result = await cmd.ExecuteScalarAsync();
        return result != null ? (Guid)result : Guid.NewGuid();
    }
}