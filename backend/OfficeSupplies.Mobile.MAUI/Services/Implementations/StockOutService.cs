using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Mobile.MAUI.Models.DTOs;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Services.Implementations;

public class StockOutService : IStockOutService
{
    private readonly IItemService _itemService;
    private readonly ITransactionService _transactionService;
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<StockOutService> _logger;

    public StockOutService(
        IItemService itemService,
        ITransactionService transactionService,
        IDepartmentService departmentService,
        ILogger<StockOutService> logger)
    {
        _itemService = itemService;
        _transactionService = transactionService;
        _departmentService = departmentService;
        _logger = logger;
    }

    public async Task<TransactionResult> ProcessStockOutAsync(StockOutRequest request)
    {
        try
        {
            _logger.LogInformation("出庫処理開始: ItemId={ItemId}, Quantity={Quantity}, DepartmentId={DepartmentId}", 
                request.ItemId, request.Quantity, request.DepartmentId);

            // 在庫確認
            var isAvailable = await ValidateStockAvailabilityAsync(request.ItemId, request.Quantity);
            if (!isAvailable)
            {
                var item = await _itemService.GetItemByIdAsync(request.ItemId);
                _logger.LogWarning("在庫不足: ItemId={ItemId}, 要求数量={Quantity}, 利用可能数量={Stock}", 
                    request.ItemId, request.Quantity, item?.CurrentStock ?? 0);
                
                return new TransactionResult
                {
                    IsSuccess = false,
                    Message = $"在庫が不足しています。利用可能数量: {item?.CurrentStock ?? 0}",
                    ErrorCode = "INSUFFICIENT_STOCK"
                };
            }

            // 在庫更新
            var currentItem = await _itemService.GetItemByIdAsync(request.ItemId);
            if (currentItem == null)
            {
                return new TransactionResult
                {
                    IsSuccess = false,
                    Message = "物品が見つかりません。",
                    ErrorCode = "ITEM_NOT_FOUND"
                };
            }

            var newStock = currentItem.CurrentStock - request.Quantity;
            var updateResult = await _itemService.UpdateStockAsync(request.ItemId, newStock);
            if (!updateResult)
            {
                return new TransactionResult
                {
                    IsSuccess = false,
                    Message = "在庫の更新に失敗しました。",
                    ErrorCode = "UPDATE_FAILED"
                };
            }

            // 取引記録作成
            var transaction = new Transaction
            {
                ItemId = request.ItemId,
                DepartmentId = request.DepartmentId,
                TransactionType = "出庫",
                Quantity = request.Quantity,
                TransactionDate = DateTime.UtcNow,
                ProcessedBy = request.ProcessedBy,
                Remarks = request.Remarks,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);

            _logger.LogInformation("出庫処理完了: TransactionId={TransactionId}, ItemId={ItemId}, Quantity={Quantity}", 
                createdTransaction.TransactionId, request.ItemId, request.Quantity);

            return new TransactionResult
            {
                IsSuccess = true,
                Message = "出庫処理が正常に完了しました。",
                Transaction = createdTransaction
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "出庫処理中にエラーが発生しました: ItemId={ItemId}", request.ItemId);
            
            return new TransactionResult
            {
                IsSuccess = false,
                Message = $"エラーが発生しました: {ex.Message}",
                ErrorCode = "SYSTEM_ERROR"
            };
        }
    }

    public async Task<TransactionResult> CancelLastTransactionAsync()
    {
        try
        {
            _logger.LogInformation("最終取引のキャンセル処理開始");
            
            var lastTransaction = await _transactionService.GetLastTransactionAsync();
            if (lastTransaction == null)
            {
                _logger.LogWarning("キャンセル可能な取引が見つかりません");
                
                return new TransactionResult
                {
                    IsSuccess = false,
                    Message = "キャンセル可能な取引が見つかりません。",
                    ErrorCode = "NO_TRANSACTION"
                };
            }

            // 在庫を戻す
            var item = await _itemService.GetItemByIdAsync(lastTransaction.ItemId);
            if (item != null)
            {
                var restoredStock = item.CurrentStock + lastTransaction.Quantity;
                await _itemService.UpdateStockAsync(lastTransaction.ItemId, restoredStock);
            }

            // 取引をキャンセル
            var cancelResult = await _transactionService.CancelTransactionAsync(
                lastTransaction.TransactionId,
                "Mobile User");

            if (cancelResult)
            {
                _logger.LogInformation("取引キャンセル完了: TransactionId={TransactionId}", lastTransaction.TransactionId);
                
                return new TransactionResult
                {
                    IsSuccess = true,
                    Message = "取引がキャンセルされました。",
                    Transaction = lastTransaction
                };
            }

            _logger.LogWarning("取引のキャンセルに失敗しました: TransactionId={TransactionId}", lastTransaction.TransactionId);
            
            return new TransactionResult
            {
                IsSuccess = false,
                Message = "取引のキャンセルに失敗しました。",
                ErrorCode = "CANCEL_FAILED"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取引キャンセル中にエラーが発生しました");
            
            return new TransactionResult
            {
                IsSuccess = false,
                Message = $"エラーが発生しました: {ex.Message}",
                ErrorCode = "SYSTEM_ERROR"
            };
        }
    }

    public async Task<bool> ValidateStockAvailabilityAsync(int itemId, int quantity)
    {
        var item = await _itemService.GetItemByIdAsync(itemId);
        if (item == null)
            return false;

        return item.CurrentStock >= quantity;
    }
}