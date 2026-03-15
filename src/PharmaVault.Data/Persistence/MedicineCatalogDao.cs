using Microsoft.Extensions.Configuration;
using Npgsql;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;
using PharmaVault.Data.Extensions;

namespace PharmaVault.Data.Persistence;

public class MedicineCatalogDao : IMedicineCatalogDao
{
    private readonly string _connectionString;

    public MedicineCatalogDao(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<IEnumerable<MedicineCatalog>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT 
                catalog_id AS CatalogId, 
                name AS Name, 
                pharmaceutical_form AS PharmaceuticalForm, 
                dosage AS Dosage, 
                is_active AS IsActive 
            FROM medicine_catalog 
            ORDER BY name ASC;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        
        return await cmd.FillToObjectListAsync<MedicineCatalog>();
    }

    public async Task<int> CreateAsync(MedicineCatalog medicine)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO medicine_catalog (name, pharmaceutical_form, dosage, is_active)
            VALUES (@Name, @PharmaceuticalForm, @Dosage, @IsActive)
            RETURNING catalog_id;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        
        cmd.Parameters.AddWithValue("Name", medicine.Name);
        cmd.Parameters.AddWithValue("PharmaceuticalForm", medicine.PharmaceuticalForm);
        cmd.Parameters.AddWithValue("Dosage", medicine.Dosage ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("IsActive", medicine.IsActive);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(MedicineCatalog medicine)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            UPDATE medicine_catalog 
            SET name = @Name, 
                pharmaceutical_form = @PharmaceuticalForm, 
                dosage = @Dosage, 
                is_active = @IsActive
            WHERE catalog_id = @CatalogId;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("Name", medicine.Name);
        cmd.Parameters.AddWithValue("PharmaceuticalForm", medicine.PharmaceuticalForm);
        cmd.Parameters.AddWithValue("Dosage", medicine.Dosage ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("IsActive", medicine.IsActive);
        cmd.Parameters.AddWithValue("CatalogId", medicine.CatalogId);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> ToggleActiveAsync(int catalogId, bool isActive)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            UPDATE medicine_catalog 
            SET is_active = @IsActive 
            WHERE catalog_id = @CatalogId;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("IsActive", isActive);
        cmd.Parameters.AddWithValue("CatalogId", catalogId);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}