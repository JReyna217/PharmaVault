using Microsoft.Extensions.Configuration;
using Npgsql;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;
using PharmaVault.Data.Extensions;

namespace PharmaVault.Data.Persistence;

public class InventoryDao : IInventoryDao
{
    private readonly string _connectionString;

    public InventoryDao(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<IEnumerable<InventoryItemDto>> GetUserInventoryAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Add ::timestamp to convert dates in correct format for DateTime property
        var sql = @"
            SELECT 
                i.inventory_id AS InventoryId,
                i.catalog_id AS CatalogId,
                mc.name AS MedicineName,
                mc.pharmaceutical_form AS PharmaceuticalForm,
                mc.dosage AS Dosage,
                i.quantity AS Quantity,
                i.purchase_date::timestamp AS PurchaseDate,
                i.expiration_date::timestamp AS ExpirationDate,
                i.prescription_notes AS PrescriptionNotes,
                i.date_added::timestamp AS DateAdded
            FROM inventory i
            INNER JOIN medicine_catalog mc ON i.catalog_id = mc.catalog_id
            WHERE i.user_id = @UserId
            ORDER BY i.expiration_date ASC;"; 

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("UserId", userId);
        
        return await cmd.FillToObjectListAsync<InventoryItemDto>();
    }

    public async Task<int> AddToInventoryAsync(Inventory inventory)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO inventory (user_id, catalog_id, quantity, purchase_date, expiration_date, prescription_notes)
            VALUES (@UserId, @CatalogId, @Quantity, @PurchaseDate, @ExpirationDate, @PrescriptionNotes)
            RETURNING inventory_id;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("UserId", inventory.UserId);
        cmd.Parameters.AddWithValue("CatalogId", inventory.CatalogId);
        cmd.Parameters.AddWithValue("Quantity", inventory.Quantity);
        cmd.Parameters.AddWithValue("PurchaseDate", inventory.PurchaseDate ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("ExpirationDate", inventory.ExpirationDate);
        cmd.Parameters.AddWithValue("PrescriptionNotes", inventory.PrescriptionNotes ?? (object)DBNull.Value);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateInventoryAsync(Inventory inventory)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            UPDATE inventory 
            SET quantity = @Quantity, 
                purchase_date = @PurchaseDate, 
                expiration_date = @ExpirationDate, 
                prescription_notes = @PrescriptionNotes
            WHERE inventory_id = @InventoryId AND user_id = @UserId;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("Quantity", inventory.Quantity);
        cmd.Parameters.AddWithValue("PurchaseDate", inventory.PurchaseDate ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("ExpirationDate", inventory.ExpirationDate);
        cmd.Parameters.AddWithValue("PrescriptionNotes", inventory.PrescriptionNotes ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("InventoryId", inventory.InventoryId);
        cmd.Parameters.AddWithValue("UserId", inventory.UserId);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteFromInventoryAsync(int inventoryId, int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "DELETE FROM inventory WHERE inventory_id = @InventoryId AND user_id = @UserId;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("InventoryId", inventoryId);
        cmd.Parameters.AddWithValue("UserId", userId);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            SELECT 
                COALESCE(SUM(quantity), 0) AS TotalStock,
                COALESCE(SUM(CASE WHEN expiration_date < CURRENT_DATE THEN quantity ELSE 0 END), 0) AS ExpiredStock,
                COALESCE(SUM(CASE WHEN expiration_date >= CURRENT_DATE AND expiration_date <= CURRENT_DATE + INTERVAL '30 days' THEN quantity ELSE 0 END), 0) AS ExpiringSoonStock,
                COALESCE(SUM(CASE WHEN expiration_date > CURRENT_DATE + INTERVAL '30 days' THEN quantity ELSE 0 END), 0) AS GoodStock
            FROM inventory
            WHERE user_id = @UserId;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("UserId", userId);

        var results = await cmd.FillToObjectListAsync<DashboardStatsDto>();
        
        return results.FirstOrDefault() ?? new DashboardStatsDto();
    }
}