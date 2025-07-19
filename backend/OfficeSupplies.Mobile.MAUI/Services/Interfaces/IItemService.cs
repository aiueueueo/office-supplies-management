using OfficeSupplies.Core.Entities;

namespace OfficeSupplies.Mobile.MAUI.Services.Interfaces;

public interface IItemService
{
    Task<Item?> GetItemByBarcodeAsync(string barcode);
    Task<Item?> GetItemByIdAsync(int itemId);
    Task<bool> UpdateStockAsync(int itemId, int newStock);
}