using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PharmaVault.Core.Interfaces;
using PharmaVault.Core.Models;
using System.Security.Claims;

namespace PharmaVault.Web.Components.Pages;

public partial class Inventory : ComponentBase
{
    [Inject] 
    public IInventoryDao InventoryDao { get; set; } = default!;
    [Inject] 
    public IMedicineCatalogDao CatalogDao { get; set; } = default!;
    [Inject] 
    public AuthenticationStateProvider AuthProvider { get; set; } = default!;

    protected IEnumerable<InventoryItemDto> _inventoryItems = default!;
    protected IEnumerable<MedicineCatalog> _catalogItems = new List<MedicineCatalog>();
    protected PharmaVault.Core.Models.Inventory _currentInventory = new();
    
    protected int _userId;
    protected bool _showModal = false;
    protected string _searchTerm = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (int.TryParse(userIdString, out int parsedId))
        {
            _userId = parsedId;
        }

        _catalogItems = await CatalogDao.GetAllAsync();
        await LoadInventoryAsync();
    }

    private async Task LoadInventoryAsync()
    {
        if (_userId > 0)
        {
            _inventoryItems = await InventoryDao.GetUserInventoryAsync(_userId);
        }
    }

    protected void PrepareForAdd()
    {
        _currentInventory = new PharmaVault.Core.Models.Inventory 
        { 
            InventoryId = 0,
            UserId = _userId, 
            ExpirationDate = DateTime.Today.AddMonths(6)
        };
        _showModal = true;
    }

    protected void PrepareForEdit(InventoryItemDto item)
    {
        _currentInventory = new PharmaVault.Core.Models.Inventory
        {
            InventoryId = item.InventoryId,
            UserId = _userId,
            CatalogId = item.CatalogId,
            Quantity = item.Quantity,
            PurchaseDate = item.PurchaseDate,
            ExpirationDate = item.ExpirationDate,
            PrescriptionNotes = item.PrescriptionNotes
        };
        _showModal = true;
    }

    protected void CloseModal()
    {
        _showModal = false;
    }

    protected async Task SaveInventoryAsync()
    {
        if (_currentInventory.CatalogId == 0) return; 

        if (_currentInventory.InventoryId == 0)
        {
            await InventoryDao.AddToInventoryAsync(_currentInventory);
        }
        else
        {
            await InventoryDao.UpdateInventoryAsync(_currentInventory);
        }

        await LoadInventoryAsync();
        _showModal = false;
    }

    protected async Task DeleteInventoryAsync(int inventoryId)
    {
        await InventoryDao.DeleteFromInventoryAsync(inventoryId, _userId);
        await LoadInventoryAsync();
    }

    protected IEnumerable<InventoryItemDto> FilteredInventory
    {
        get
        {
            if (_inventoryItems == null)
                return Enumerable.Empty<InventoryItemDto>();
            
            if (string.IsNullOrWhiteSpace(_searchTerm))
                return _inventoryItems;

            return _inventoryItems.Where(i => 
                i.MedicineName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (i.PharmaceuticalForm != null && i.PharmaceuticalForm.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))
            );
        }
    }
}