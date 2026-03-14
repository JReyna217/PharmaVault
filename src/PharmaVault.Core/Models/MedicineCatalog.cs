using System;

namespace PharmaVault.Core.Models;

public class MedicineCatalog
{
    public int CatalogId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PharmaceuticalForm { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public bool IsActive { get; set; } = true;
}
