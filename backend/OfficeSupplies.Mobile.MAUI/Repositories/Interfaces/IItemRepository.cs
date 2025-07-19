using OfficeSupplies.Core.Entities;

namespace OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;

public interface IItemRepository : IBaseRepository<Item>
{
    Task<Item?> GetByBarcodeAsync(string barcode);
    Task<bool> UpdateStockAsync(int itemId, int newStock);
}