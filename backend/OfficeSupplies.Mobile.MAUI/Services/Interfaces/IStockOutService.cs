using OfficeSupplies.Mobile.MAUI.Models.DTOs;

namespace OfficeSupplies.Mobile.MAUI.Services.Interfaces;

public interface IStockOutService
{
    Task<TransactionResult> ProcessStockOutAsync(StockOutRequest request);
    Task<TransactionResult> CancelLastTransactionAsync();
    Task<bool> ValidateStockAvailabilityAsync(int itemId, int quantity);
}