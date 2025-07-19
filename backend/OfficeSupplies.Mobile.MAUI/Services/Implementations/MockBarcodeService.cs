using Microsoft.Extensions.Logging;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Services.Implementations;

public class MockBarcodeService : IBarcodeService
{
    private readonly ILogger<MockBarcodeService> _logger;
    private static readonly string[] SampleBarcodes = {
        "ITEM001",
        "ITEM002", 
        "ITEM003",
        "ITEM004",
        "ITEM005"
    };

    public MockBarcodeService(ILogger<MockBarcodeService> logger)
    {
        _logger = logger;
    }

    public async Task<string> ScanBarcodeAsync()
    {
        _logger.LogInformation("モックバーコードスキャン開始");
        
        // 実際のカメラスキャンをシミュレート
        await Task.Delay(1000);

        // ランダムなサンプルバーコードを返す
        var random = new Random();
        var barcode = SampleBarcodes[random.Next(SampleBarcodes.Length)];
        
        _logger.LogInformation("モックバーコードスキャン完了: {Barcode}", barcode);
        
        return barcode;
    }

    public bool ValidateBarcodeFormat(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            _logger.LogWarning("バーコードが空です");
            return false;
        }

        // 基本的なバーコード形式チェック（英数字、3-20文字）
        var isValid = System.Text.RegularExpressions.Regex.IsMatch(barcode, @"^[A-Za-z0-9]{3,20}$");
        
        if (!isValid)
        {
            _logger.LogWarning("無効なバーコード形式: {Barcode}", barcode);
        }
        
        return isValid;
    }
}