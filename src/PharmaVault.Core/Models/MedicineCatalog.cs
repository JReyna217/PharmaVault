using System;
using System.ComponentModel.DataAnnotations;

namespace PharmaVault.Core.Models;

public class MedicineCatalog
{
    public int CatalogId { get; set; }

    [Required(ErrorMessage = "The medicine name is required.")]
    [StringLength(250, ErrorMessage = "The name cannot exceed 250 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "The pharmaceutical form is required.")]
    [StringLength(50, ErrorMessage = "The form cannot exceed 50 characters.")]
    public string PharmaceuticalForm { get; set; } = string.Empty;
    
    [StringLength(50, ErrorMessage = "The dosage cannot exceed 50 characters.")]
    public string? Dosage { get; set; }
    public bool IsActive { get; set; } = true;
}
