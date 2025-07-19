using OfficeSupplies.Core.Entities;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Services.Implementations;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Item?> GetItemByBarcodeAsync(string barcode)
    {
        return await _itemRepository.GetByBarcodeAsync(barcode);
    }

    public async Task<Item?> GetItemByIdAsync(int itemId)
    {
        return await _itemRepository.GetByIdAsync(itemId);
    }

    public async Task<bool> UpdateStockAsync(int itemId, int newStock)
    {
        return await _itemRepository.UpdateStockAsync(itemId, newStock);
    }
}