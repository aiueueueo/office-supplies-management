using OfficeSupplies.Core.Entities;

namespace OfficeSupplies.Mobile.MAUI.Models.DTOs;

public class TransactionResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Transaction? Transaction { get; set; }
    public string? ErrorCode { get; set; }
}