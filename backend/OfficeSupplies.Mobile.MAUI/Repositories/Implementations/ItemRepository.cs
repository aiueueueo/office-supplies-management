using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Infrastructure.Data;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Repositories.Implementations;

public class ItemRepository : BaseRepository<Item>, IItemRepository
{
    private readonly ILogger<ItemRepository> _logger;

    public ItemRepository(OfficeSuppliesContext context, ILogger<ItemRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<Item?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.ItemCode == barcode && i.IsActive);
    }

    public async Task<bool> UpdateStockAsync(int itemId, int newStock)
    {
        const int maxRetries = 3;
        int retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                var item = await GetByIdAsync(itemId);
                if (item == null)
                {
                    _logger.LogWarning("物品が見つかりません: ItemId={ItemId}", itemId);
                    return false;
                }

                item.CurrentStock = newStock;
                item.UpdatedAt = DateTime.Now;

                await UpdateAsync(item);
                var result = await SaveChangesAsync();
                
                if (result > 0)
                {
                    _logger.LogInformation("在庫更新成功: ItemId={ItemId}, NewStock={NewStock}", itemId, newStock);
                    return true;
                }
                
                return false;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryCount++;
                _logger.LogWarning(ex, "在庫更新で同時実行エラーが発生しました。リトライ {RetryCount}/{MaxRetries}", retryCount, maxRetries);
                
                if (retryCount >= maxRetries)
                {
                    _logger.LogError(ex, "在庫更新が最大リトライ回数を超えました: ItemId={ItemId}", itemId);
                    throw;
                }
                
                // リトライ前に少し待機
                await Task.Delay(100 * retryCount);
            }
        }
        
        return false;
    }
}