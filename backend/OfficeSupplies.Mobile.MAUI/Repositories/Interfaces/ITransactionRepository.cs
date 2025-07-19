using OfficeSupplies.Core.Entities;

namespace OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
    Task<Transaction?> GetLastTransactionAsync();
    Task<bool> CancelTransactionAsync(int transactionId, string cancelledBy);
}