using Microsoft.EntityFrameworkCore;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Mobile.MAUI.Models.DTOs;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Services.Implementations;

public class StockOutService : IStockOutService
{
    private readonly IItemService _itemService;
    private readonly ITransactionService _transactionService;
    private readonly IDepartmentService _departmentService;

    public StockOutService(
        IItemService itemService,
        ITransactionService transactionService,
        IDepartmentService departmentService)
    {
        _itemService = itemService;
        _transactionService = transactionService;
        _departmentService = departmentService;
    }

    public async Task<TransactionResult> ProcessStockOutAsync(StockOutRequest request)
    {
        try
        {
            // 在庫確認
            var isAvailable = await ValidateStockAvailabilityAsync(request.ItemId, request.Quantity);
            if (!isAvailable)
            {
                var item = await _itemService.GetItemByIdAsync(request.ItemId);
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
                TransactionDate = DateTime.Now,
                ProcessedBy = request.ProcessedBy,
                Remarks = request.Remarks,
                IsCancelled = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);

            return new TransactionResult
            {
                IsSuccess = true,
                Message = "出庫処理が正常に完了しました。",
                Transaction = createdTransaction
            };
        }
        catch (Exception ex)
        {
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
            var lastTransaction = await _transactionService.GetLastTransactionAsync();
            if (lastTransaction == null)
            {
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
                return new TransactionResult
                {
                    IsSuccess = true,
                    Message = "取引がキャンセルされました。",
                    Transaction = lastTransaction
                };
            }

            return new TransactionResult
            {
                IsSuccess = false,
                Message = "取引のキャンセルに失敗しました。",
                ErrorCode = "CANCEL_FAILED"
            };
        }
        catch (Exception ex)
        {
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