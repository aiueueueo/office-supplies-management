using Microsoft.Extensions.Logging;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Services.Implementations;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ItemService> _logger;

    public ItemService(
        IItemRepository itemRepository,
        ILogger<ItemService> logger)
    {
        _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Item?> GetItemByBarcodeAsync(string barcode)
    {
        try
        {
            // ビジネスルール: バーコードは必須で、空白文字のみも無効
            if (string.IsNullOrWhiteSpace(barcode))
            {
                _logger.LogWarning("無効なバーコード: 空または空白文字のみ");
                throw new ArgumentException("バーコードは必須です。", nameof(barcode));
            }

            // ビジネスルール: バーコードは3-20文字の英数字のみ
            if (!System.Text.RegularExpressions.Regex.IsMatch(barcode, @"^[A-Za-z0-9]{3,20}$"))
            {
                _logger.LogWarning("無効なバーコード形式: {Barcode}", barcode);
                throw new ArgumentException("バーコードは3-20文字の英数字である必要があります。", nameof(barcode));
            }

            _logger.LogInformation("バーコード検索開始: {Barcode}", barcode);
            
            var item = await _itemRepository.GetByBarcodeAsync(barcode);
            
            if (item == null)
            {
                _logger.LogWarning("バーコードに対応する物品が見つかりません: {Barcode}", barcode);
                return null;
            }

            // ビジネスルール: 非アクティブな物品は返さない
            if (!item.IsActive)
            {
                _logger.LogWarning("非アクティブな物品が検索されました: ItemId={ItemId}, Barcode={Barcode}", 
                    item.ItemId, barcode);
                return null;
            }

            _logger.LogInformation("物品検索完了: ItemId={ItemId}, Name={ItemName}", 
                item.ItemId, item.ItemName);
            
            return item;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "バーコード検索に失敗しました: {Barcode}", barcode);
            throw new InvalidOperationException($"バーコード({barcode})の検索に失敗しました。", ex);
        }
    }

    public async Task<Item?> GetItemByIdAsync(int itemId)
    {
        try
        {
            // ビジネスルール: IDは正の値である必要がある
            if (itemId <= 0)
            {
                _logger.LogWarning("無効な物品ID: {ItemId}", itemId);
                throw new ArgumentException("物品IDは正の値である必要があります。", nameof(itemId));
            }

            _logger.LogInformation("物品取得開始: ItemId={ItemId}", itemId);
            
            var item = await _itemRepository.GetByIdAsync(itemId);
            
            if (item == null)
            {
                _logger.LogWarning("物品が見つかりません: ItemId={ItemId}", itemId);
                return null;
            }

            // ビジネスルール: 非アクティブな物品は返さない
            if (!item.IsActive)
            {
                _logger.LogWarning("非アクティブな物品が要求されました: ItemId={ItemId}", itemId);
                return null;
            }

            _logger.LogInformation("物品取得完了: ItemId={ItemId}, Name={ItemName}", 
                item.ItemId, item.ItemName);
            
            return item;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "物品の取得に失敗しました: ItemId={ItemId}", itemId);
            throw new InvalidOperationException($"物品(ID: {itemId})の取得に失敗しました。", ex);
        }
    }

    public async Task<bool> UpdateStockAsync(int itemId, int newStock)
    {
        try
        {
            // ビジネスルール: IDは正の値である必要がある
            if (itemId <= 0)
            {
                _logger.LogWarning("無効な物品ID: {ItemId}", itemId);
                throw new ArgumentException("物品IDは正の値である必要があります。", nameof(itemId));
            }

            // ビジネスルール: 在庫数は0以上である必要がある
            if (newStock < 0)
            {
                _logger.LogWarning("無効な在庫数: {NewStock}", newStock);
                throw new ArgumentException("在庫数は0以上である必要があります。", nameof(newStock));
            }

            // ビジネスルール: 在庫数の上限チェック（999,999個まで）
            if (newStock > 999999)
            {
                _logger.LogWarning("在庫数が上限を超えています: {NewStock}", newStock);
                throw new ArgumentException("在庫数は999,999個以下である必要があります。", nameof(newStock));
            }

            _logger.LogInformation("在庫更新開始: ItemId={ItemId}, NewStock={NewStock}", itemId, newStock);
            
            // 物品の存在とアクティブ状態を確認
            var item = await GetItemByIdAsync(itemId);
            if (item == null)
            {
                _logger.LogWarning("在庫更新対象の物品が見つかりません: ItemId={ItemId}", itemId);
                throw new InvalidOperationException($"物品(ID: {itemId})が見つかりません。");
            }

            var result = await _itemRepository.UpdateStockAsync(itemId, newStock);
            
            if (result)
            {
                _logger.LogInformation("在庫更新完了: ItemId={ItemId}, NewStock={NewStock}", itemId, newStock);
            }
            else
            {
                _logger.LogWarning("在庫更新に失敗しました: ItemId={ItemId}, NewStock={NewStock}", itemId, newStock);
            }
            
            return result;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "在庫更新に失敗しました: ItemId={ItemId}, NewStock={NewStock}", itemId, newStock);
            throw new InvalidOperationException($"物品(ID: {itemId})の在庫更新に失敗しました。", ex);
        }
    }
}