using System;
using PharmaVault.Core.Models;

namespace PharmaVault.Core.Interfaces;

public interface IMedicineCatalogDao
{
    Task<IEnumerable<MedicineCatalog>> GetAllAsync();
    Task<int> CreateAsync(MedicineCatalog medicine);
    Task<bool> UpdateAsync(MedicineCatalog medicine);
    Task<bool> ToggleActiveAsync(int catalogId, bool isActive);
}
