namespace OfficeSupplies.Mobile.MAUI.Models.DTOs;

public class StockOutRequest
{
    public int ItemId { get; set; }
    public int DepartmentId { get; set; }
    public int Quantity { get; set; }
    public string ProcessedBy { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}