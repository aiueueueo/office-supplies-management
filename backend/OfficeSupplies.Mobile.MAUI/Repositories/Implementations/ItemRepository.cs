using Microsoft.EntityFrameworkCore;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Infrastructure.Data;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Repositories.Implementations;

public class ItemRepository : BaseRepository<Item>, IItemRepository
{
    public ItemRepository(OfficeSuppliesContext context) : base(context)
    {
    }

    public async Task<Item?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.ItemCode == barcode && i.IsActive);
    }

    public async Task<bool> UpdateStockAsync(int itemId, int newStock)
    {
        var item = await GetByIdAsync(itemId);
        if (item == null)
            return false;

        item.CurrentStock = newStock;
        item.UpdatedAt = DateTime.Now;

        await UpdateAsync(item);
        var result = await SaveChangesAsync();
        return result > 0;
    }
}