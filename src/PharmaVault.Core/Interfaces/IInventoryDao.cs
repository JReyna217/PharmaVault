using PharmaVault.Core.Models;

namespace PharmaVault.Core.Interfaces;

public interface IInventoryDao
{
    Task<IEnumerable<InventoryItemDto>> GetUserInventoryAsync(int userId);
    Task<int> AddToInventoryAsync(Inventory inventory);
    Task<bool> UpdateInventoryAsync(Inventory inventory);
    Task<bool> DeleteFromInventoryAsync(int inventoryId, int userId);
    Task<DashboardStatsDto> GetDashboardStatsAsync(int userId);
}