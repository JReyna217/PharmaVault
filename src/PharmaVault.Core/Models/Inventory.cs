using System;

namespace PharmaVault.Core.Models;

public class Inventory
{
    public int InventoryId { get; set; }
    public int UserId { get; set; }
    public int CatalogId { get; set; }
    public int Quantity { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string? PrescriptionNotes { get; set; }
    public DateTime DateAdded { get; set; }
}
