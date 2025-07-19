using OfficeSupplies.Core.Entities;

namespace OfficeSupplies.Mobile.MAUI.Services.Interfaces;

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<Transaction?> GetLastTransactionAsync();
    Task<bool> CancelTransactionAsync(int transactionId, string cancelledBy);
}