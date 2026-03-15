using System;
using System.ComponentModel.DataAnnotations;

namespace PharmaVault.Core.Models;

public class InventoryItemDto
{
    public int InventoryId { get; set; }
    public int CatalogId { get; set; }
    
    public string MedicineName { get; set; } = string.Empty;
    public string PharmaceuticalForm { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    
    public int Quantity { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string? PrescriptionNotes { get; set; }
    public DateTime DateAdded { get; set; }
    public bool IsExpired => ExpirationDate < DateTime.Today;
}