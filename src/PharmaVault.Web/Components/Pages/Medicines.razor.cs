using Microsoft.AspNetCore.Components;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;

namespace PharmaVault.Web.Components.Pages;

public partial class Medicines : ComponentBase
{
    [Inject] 
    public IMedicineCatalogDao MedicineCatalogDao { get; set; } = default!;

    private List<MedicineCatalog>? _medicines;
    private MedicineCatalog _currentMedicine = new();
    
    private bool _showModal = false;
    private bool _isEditing = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadMedicinesAsync();
    }

    private async Task LoadMedicinesAsync()
    {
        var result = await MedicineCatalogDao.GetAllAsync();
        _medicines = result.ToList();
    }

    private void OpenModal(MedicineCatalog? medicine = null)
    {
        if (medicine != null)
        {
            _isEditing = true;
            _currentMedicine = new MedicineCatalog
            {
                CatalogId = medicine.CatalogId,
                Name = medicine.Name,
                PharmaceuticalForm = medicine.PharmaceuticalForm,
                Dosage = medicine.Dosage,
                IsActive = medicine.IsActive
            };
        }
        else
        {
            _isEditing = false;
            _currentMedicine = new MedicineCatalog { IsActive = true };
        }
        
        _showModal = true;
    }

    private void CloseModal()
    {
        _showModal = false;
        _currentMedicine = new MedicineCatalog();
    }

    private async Task SaveMedicineAsync()
    {
        if (_isEditing)
        {
            await MedicineCatalogDao.UpdateAsync(_currentMedicine);
        }
        else
        {
            await MedicineCatalogDao.CreateAsync(_currentMedicine);
        }

        await LoadMedicinesAsync();
        CloseModal();
    }

    private async Task ToggleStatusAsync(int catalogId, bool currentStatus)
    {
        await MedicineCatalogDao.ToggleActiveAsync(catalogId, !currentStatus);
        await LoadMedicinesAsync();
    }
}