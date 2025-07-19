using OfficeSupplies.Core.Entities;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        transaction.CreatedAt = DateTime.Now;
        transaction.UpdatedAt = DateTime.Now;
        transaction.TransactionDate = DateTime.Now;

        var result = await _transactionRepository.AddAsync(transaction);
        await _transactionRepository.SaveChangesAsync();
        return result;
    }

    public async Task<Transaction?> GetLastTransactionAsync()
    {
        return await _transactionRepository.GetLastTransactionAsync();
    }

    public async Task<bool> CancelTransactionAsync(int transactionId, string cancelledBy)
    {
        return await _transactionRepository.CancelTransactionAsync(transactionId, cancelledBy);
    }
}