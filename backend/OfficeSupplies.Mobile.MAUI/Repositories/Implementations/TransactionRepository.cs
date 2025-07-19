using Microsoft.EntityFrameworkCore;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Infrastructure.Data;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Repositories.Implementations;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(OfficeSuppliesContext context) : base(context)
    {
    }

    public async Task<Transaction?> GetLastTransactionAsync()
    {
        return await _dbSet
            .Include(t => t.Item)
            .Include(t => t.Department)
            .Where(t => t.TransactionType == "出庫" && !t.IsCancelled)
            .OrderByDescending(t => t.TransactionDate)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CancelTransactionAsync(int transactionId, string cancelledBy)
    {
        var transaction = await GetByIdAsync(transactionId);
        if (transaction == null || transaction.IsCancelled)
            return false;

        transaction.IsCancelled = true;
        transaction.CancelledBy = cancelledBy;
        transaction.CancelledAt = DateTime.Now;
        transaction.UpdatedAt = DateTime.Now;

        await UpdateAsync(transaction);
        var result = await SaveChangesAsync();
        return result > 0;
    }
}