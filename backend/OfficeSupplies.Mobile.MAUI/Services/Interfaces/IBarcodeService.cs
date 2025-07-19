namespace OfficeSupplies.Mobile.MAUI.Services.Interfaces;

public interface IBarcodeService
{
    Task<string> ScanBarcodeAsync();
    bool ValidateBarcodeFormat(string barcode);
}