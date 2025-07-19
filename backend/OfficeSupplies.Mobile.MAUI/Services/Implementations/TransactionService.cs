using Microsoft.Extensions.Logging;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        ITransactionRepository transactionRepository,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        try
        {
            // ビジネスルール: Transactionオブジェクトは必須
            if (transaction == null)
            {
                _logger.LogWarning("Transactionオブジェクトがnullです");
                throw new ArgumentNullException(nameof(transaction), "取引情報は必須です。");
            }

            // ビジネスルール: 必須フィールドの検証
            ValidateTransactionFields(transaction);

            // ビジネスルール: 出庫数量は正の値である必要がある
            if (transaction.Quantity <= 0)
            {
                _logger.LogWarning("無効な出庫数量: {Quantity}", transaction.Quantity);
                throw new ArgumentException("出庫数量は正の値である必要があります。", nameof(transaction));
            }

            // ビジネスルール: 出庫数量の上限チェック（9999個まで）
            if (transaction.Quantity > 9999)
            {
                _logger.LogWarning("出庫数量が上限を超えています: {Quantity}", transaction.Quantity);
                throw new ArgumentException("出庫数量は9999個以下である必要があります。", nameof(transaction));
            }

            // ビジネスルール: 処理者名は必須
            if (string.IsNullOrWhiteSpace(transaction.ProcessedBy))
            {
                _logger.LogWarning("処理者名が未設定です");
                throw new ArgumentException("処理者名は必須です。", nameof(transaction));
            }

            _logger.LogInformation("取引作成開始: ItemId={ItemId}, DepartmentId={DepartmentId}, Quantity={Quantity}", 
                transaction.ItemId, transaction.DepartmentId, transaction.Quantity);

            // タイムスタンプの設定
            var now = DateTime.Now;
            transaction.CreatedAt = now;
            transaction.UpdatedAt = now;
            transaction.TransactionDate = now;
            transaction.IsCancelled = false;

            var result = await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            _logger.LogInformation("取引作成完了: TransactionId={TransactionId}", result.TransactionId);
            
            return result;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取引の作成に失敗しました");
            throw new InvalidOperationException("取引の作成に失敗しました。", ex);
        }
    }

    public async Task<Transaction?> GetLastTransactionAsync()
    {
        try
        {
            _logger.LogInformation("最新取引の取得を開始");
            
            var transaction = await _transactionRepository.GetLastTransactionAsync();
            
            if (transaction == null)
            {
                _logger.LogInformation("最新の取引が見つかりません");
                return null;
            }

            _logger.LogInformation("最新取引取得完了: TransactionId={TransactionId}", transaction.TransactionId);
            
            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "最新取引の取得に失敗しました");
            throw new InvalidOperationException("最新取引の取得に失敗しました。", ex);
        }
    }

    public async Task<bool> CancelTransactionAsync(int transactionId, string cancelledBy)
    {
        try
        {
            // ビジネスルール: IDは正の値である必要がある
            if (transactionId <= 0)
            {
                _logger.LogWarning("無効な取引ID: {TransactionId}", transactionId);
                throw new ArgumentException("取引IDは正の値である必要があります。", nameof(transactionId));
            }

            // ビジネスルール: キャンセル者名は必須
            if (string.IsNullOrWhiteSpace(cancelledBy))
            {
                _logger.LogWarning("キャンセル者名が未設定です");
                throw new ArgumentException("キャンセル者名は必須です。", nameof(cancelledBy));
            }

            _logger.LogInformation("取引キャンセル開始: TransactionId={TransactionId}, CancelledBy={CancelledBy}", 
                transactionId, cancelledBy);

            var result = await _transactionRepository.CancelTransactionAsync(transactionId, cancelledBy);
            
            if (result)
            {
                _logger.LogInformation("取引キャンセル完了: TransactionId={TransactionId}", transactionId);
            }
            else
            {
                _logger.LogWarning("取引キャンセルに失敗しました: TransactionId={TransactionId}", transactionId);
            }
            
            return result;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取引のキャンセルに失敗しました: TransactionId={TransactionId}", transactionId);
            throw new InvalidOperationException($"取引(ID: {transactionId})のキャンセルに失敗しました。", ex);
        }
    }

    private void ValidateTransactionFields(Transaction transaction)
    {
        // ビジネスルール: ItemIdは正の値である必要がある
        if (transaction.ItemId <= 0)
        {
            _logger.LogWarning("無効な物品ID: {ItemId}", transaction.ItemId);
            throw new ArgumentException("物品IDは正の値である必要があります。");
        }

        // ビジネスルール: DepartmentIdは正の値である必要がある
        if (transaction.DepartmentId <= 0)
        {
            _logger.LogWarning("無効な部署ID: {DepartmentId}", transaction.DepartmentId);
            throw new ArgumentException("部署IDは正の値である必要があります。");
        }

        // ビジネスルール: TransactionTypeは必須
        if (string.IsNullOrWhiteSpace(transaction.TransactionType))
        {
            _logger.LogWarning("取引種別が未設定です");
            throw new ArgumentException("取引種別は必須です。");
        }

        // ビジネスルール: TransactionTypeは「出庫」または「入庫」のみ
        if (transaction.TransactionType != "出庫" && transaction.TransactionType != "入庫")
        {
            _logger.LogWarning("無効な取引種別: {TransactionType}", transaction.TransactionType);
            throw new ArgumentException("取引種別は「出庫」または「入庫」である必要があります。");
        }

        // ビジネスルール: 備考の長さ制限（500文字まで）
        if (!string.IsNullOrEmpty(transaction.Remarks) && transaction.Remarks.Length > 500)
        {
            _logger.LogWarning("備考が長すぎます: {Length}文字", transaction.Remarks.Length);
            throw new ArgumentException("備考は500文字以下である必要があります。");
        }
    }
}