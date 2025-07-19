using System.ComponentModel.DataAnnotations;

namespace OfficeSupplies.Mobile.MAUI.Models.DTOs;

public class StockOutRequest
{
    [Required(ErrorMessage = "物品IDは必須です")]
    [Range(1, int.MaxValue, ErrorMessage = "物品IDは1以上である必要があります")]
    public int ItemId { get; set; }

    [Required(ErrorMessage = "部署IDは必須です")]
    [Range(1, int.MaxValue, ErrorMessage = "部署IDは1以上である必要があります")]
    public int DepartmentId { get; set; }

    [Required(ErrorMessage = "数量は必須です")]
    [Range(1, 9999, ErrorMessage = "数量は1から9999の間である必要があります")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "処理者は必須です")]
    [StringLength(100, ErrorMessage = "処理者名は100文字以内である必要があります")]
    public string ProcessedBy { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "備考は500文字以内である必要があります")]
    public string? Remarks { get; set; }
}