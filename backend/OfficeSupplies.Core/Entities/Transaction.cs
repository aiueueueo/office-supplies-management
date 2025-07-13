namespace OfficeSupplies.Core.Entities;

public class Transaction
{
    public int TransactionId { get; set; }
    public int ItemId { get; set; }
    public int DepartmentId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // "IN" or "OUT"
    public int Quantity { get; set; }
    public int BeforeStock { get; set; }
    public int AfterStock { get; set; }
    public string? Remarks { get; set; }
    public string ProcessedBy { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    
    public Item Item { get; set; } = null!;
    public Department Department { get; set; } = null!;
}