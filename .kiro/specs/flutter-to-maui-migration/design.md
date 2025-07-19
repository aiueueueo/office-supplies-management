# 設計ドキュメント

## 概要

Flutter出庫端末アプリを.NET MAUIに移行し、直接データベース接続を実現するシステムの設計です。既存のEntity Frameworkモデルを活用し、ビジネスロジックとUI層を分離した再利用可能なアーキテクチャを構築します。

## アーキテクチャ

### システム全体構成

```
┌─────────────────────────────────────────────────────────────┐
│                    .NET MAUI Mobile App                     │
├─────────────────────────────────────────────────────────────┤
│  UI Layer (Views/Pages)                                     │
│  ├─ HomePage                                                │
│  ├─ DepartmentSelectionPage                                 │
│  ├─ BarcodeScannerPage                                      │
│  ├─ QuantityInputPage                                       │
│  ├─ ConfirmationPage                                        │
│  └─ ResultPage                                              │
├─────────────────────────────────────────────────────────────┤
│  Presentation Layer (ViewModels)                            │
│  ├─ HomeViewModel                                           │
│  ├─ DepartmentSelectionViewModel                            │
│  ├─ BarcodeScannerViewModel                                 │
│  ├─ QuantityInputViewModel                                  │
│  ├─ ConfirmationViewModel                                   │
│  └─ ResultViewModel                                         │
├─────────────────────────────────────────────────────────────┤
│  Business Logic Layer (Services)                            │
│  ├─ IStockOutService / StockOutService                      │
│  ├─ IDepartmentService / DepartmentService                  │
│  ├─ IItemService / ItemService                              │
│  ├─ ITransactionService / TransactionService                │
│  └─ IBarcodeService / BarcodeService                        │
├─────────────────────────────────────────────────────────────┤
│  Data Access Layer (Repositories)                           │
│  ├─ IDepartmentRepository / DepartmentRepository            │
│  ├─ IItemRepository / ItemRepository                        │
│  └─ ITransactionRepository / TransactionRepository          │
├─────────────────────────────────────────────────────────────┤
│  Infrastructure Layer                                       │
│  ├─ OfficeSupplies.Infrastructure (既存)                    │
│  ├─ OfficeSupplies.Core (既存)                              │
│  └─ Entity Framework Core                                   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │  SQL Server DB  │
                    │  ├─ Departments │
                    │  ├─ Items       │
                    │  └─ Transactions│
                    └─────────────────┘
```

### プロジェクト構成

```
OfficeSupplies.Mobile.MAUI/
├─ Platforms/
│  ├─ Android/
│  ├─ iOS/
│  └─ Windows/
├─ Views/
│  ├─ HomePage.xaml
│  ├─ DepartmentSelectionPage.xaml
│  ├─ BarcodeScannerPage.xaml
│  ├─ QuantityInputPage.xaml
│  ├─ ConfirmationPage.xaml
│  └─ ResultPage.xaml
├─ ViewModels/
│  ├─ BaseViewModel.cs
│  ├─ HomeViewModel.cs
│  ├─ DepartmentSelectionViewModel.cs
│  ├─ BarcodeScannerViewModel.cs
│  ├─ QuantityInputViewModel.cs
│  ├─ ConfirmationViewModel.cs
│  └─ ResultViewModel.cs
├─ Services/
│  ├─ Interfaces/
│  │  ├─ IStockOutService.cs
│  │  ├─ IDepartmentService.cs
│  │  ├─ IItemService.cs
│  │  ├─ ITransactionService.cs
│  │  └─ IBarcodeService.cs
│  └─ Implementations/
│     ├─ StockOutService.cs
│     ├─ DepartmentService.cs
│     ├─ ItemService.cs
│     ├─ TransactionService.cs
│     └─ BarcodeService.cs
├─ Repositories/
│  ├─ Interfaces/
│  │  ├─ IDepartmentRepository.cs
│  │  ├─ IItemRepository.cs
│  │  └─ ITransactionRepository.cs
│  └─ Implementations/
│     ├─ DepartmentRepository.cs
│     ├─ ItemRepository.cs
│     └─ TransactionRepository.cs
├─ Models/
│  ├─ ViewModels/
│  │  ├─ DepartmentViewModel.cs
│  │  ├─ ItemViewModel.cs
│  │  └─ TransactionViewModel.cs
│  └─ DTOs/
│     ├─ StockOutRequest.cs
│     └─ TransactionResult.cs
├─ Utilities/
│  ├─ NavigationService.cs
│  ├─ DialogService.cs
│  └─ ValidationHelper.cs
└─ MauiProgram.cs

参照プロジェクト:
├─ OfficeSupplies.Core (既存)
└─ OfficeSupplies.Infrastructure (既存)
```

## コンポーネントと インターフェース

### 1. UI Layer (Views/Pages)

#### HomePage
- **責任**: アプリケーションのエントリーポイント
- **機能**: 出庫処理開始ボタン、使用方法表示
- **ナビゲーション**: DepartmentSelectionPageへ遷移

#### DepartmentSelectionPage
- **責任**: 部署選択UI
- **機能**: アクティブな部署一覧表示、部署選択
- **データバインディング**: DepartmentSelectionViewModel

#### BarcodeScannerPage
- **責任**: バーコード/QRコードスキャンUI
- **機能**: カメラ起動、コードスキャン、手動入力オプション
- **プラットフォーム依存**: カメラアクセス

#### QuantityInputPage
- **責任**: 出庫数量入力UI
- **機能**: 物品情報表示、数量入力、在庫チェック
- **バリデーション**: 数量範囲チェック

#### ConfirmationPage
- **責任**: 取引確認UI
- **機能**: 取引詳細表示、最終確認
- **データ**: 部署、物品、数量情報

#### ResultPage
- **責任**: 処理結果表示UI
- **機能**: 成功/失敗表示、取消オプション、次の操作選択

### 2. Presentation Layer (ViewModels)

#### BaseViewModel
```csharp
public abstract class BaseViewModel : INotifyPropertyChanged
{
    protected bool _isBusy;
    public bool IsBusy { get; set; }
    
    protected string _title;
    public string Title { get; set; }
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null);
    protected virtual bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "");
}
```

#### DepartmentSelectionViewModel
```csharp
public class DepartmentSelectionViewModel : BaseViewModel
{
    private readonly IDepartmentService _departmentService;
    
    public ObservableCollection<DepartmentViewModel> Departments { get; set; }
    public DepartmentViewModel SelectedDepartment { get; set; }
    public ICommand LoadDepartmentsCommand { get; }
    public ICommand SelectDepartmentCommand { get; }
}
```

#### BarcodeScannerViewModel
```csharp
public class BarcodeScannerViewModel : BaseViewModel
{
    private readonly IBarcodeService _barcodeService;
    private readonly IItemService _itemService;
    
    public string ScannedCode { get; set; }
    public ItemViewModel ScannedItem { get; set; }
    public ICommand ScanBarcodeCommand { get; }
    public ICommand ManualInputCommand { get; }
}
```

### 3. Business Logic Layer (Services)

#### IStockOutService
```csharp
public interface IStockOutService
{
    Task<TransactionResult> ProcessStockOutAsync(StockOutRequest request);
    Task<TransactionResult> CancelLastTransactionAsync();
    Task<bool> ValidateStockAvailabilityAsync(int itemId, int quantity);
}
```

#### IDepartmentService
```csharp
public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
    Task<Department> GetDepartmentByIdAsync(int departmentId);
}
```

#### IItemService
```csharp
public interface IItemService
{
    Task<Item> GetItemByBarcodeAsync(string barcode);
    Task<Item> GetItemByIdAsync(int itemId);
    Task<bool> UpdateStockAsync(int itemId, int newStock);
}
```

#### ITransactionService
```csharp
public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<Transaction> GetLastTransactionAsync();
    Task<bool> CancelTransactionAsync(int transactionId, string cancelledBy);
}
```

#### IBarcodeService
```csharp
public interface IBarcodeService
{
    Task<string> ScanBarcodeAsync();
    bool ValidateBarcodeFormat(string barcode);
}
```

### 4. Data Access Layer (Repositories)

#### IDepartmentRepository
```csharp
public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
    Task<Department> GetByIdAsync(int id);
}
```

#### IItemRepository
```csharp
public interface IItemRepository
{
    Task<Item> GetByBarcodeAsync(string barcode);
    Task<Item> GetByIdAsync(int id);
    Task<bool> UpdateAsync(Item item);
}
```

#### ITransactionRepository
```csharp
public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> GetLastTransactionAsync();
    Task<bool> UpdateAsync(Transaction transaction);
}
```

## データモデル

### ViewModels

#### DepartmentViewModel
```csharp
public class DepartmentViewModel
{
    public int DepartmentId { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
    public string DisplayName => $"{DepartmentCode} - {DepartmentName}";
}
```

#### ItemViewModel
```csharp
public class ItemViewModel
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public string Unit { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public bool IsLowStock => CurrentStock <= MinimumStock;
    public string StockStatus => IsLowStock ? "在庫少" : "在庫あり";
}
```

### DTOs

#### StockOutRequest
```csharp
public class StockOutRequest
{
    public int ItemId { get; set; }
    public int DepartmentId { get; set; }
    public int Quantity { get; set; }
    public string ProcessedBy { get; set; }
    public string Remarks { get; set; }
}
```

#### TransactionResult
```csharp
public class TransactionResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public Transaction Transaction { get; set; }
    public string ErrorCode { get; set; }
}
```

## エラーハンドリング

### エラー分類

#### 1. データベース接続エラー
```csharp
public class DatabaseConnectionException : Exception
{
    public DatabaseConnectionException(string message, Exception innerException) 
        : base(message, innerException) { }
}
```

#### 2. バーコードスキャンエラー
```csharp
public class BarcodeScanException : Exception
{
    public BarcodeScanException(string message) : base(message) { }
}
```

#### 3. 在庫不足エラー
```csharp
public class InsufficientStockException : Exception
{
    public int RequestedQuantity { get; }
    public int AvailableStock { get; }
    
    public InsufficientStockException(int requested, int available) 
        : base($"在庫不足: 要求数量 {requested}, 利用可能数量 {available}")
    {
        RequestedQuantity = requested;
        AvailableStock = available;
    }
}
```

### エラーハンドリング戦略

#### グローバルエラーハンドラー
```csharp
public class GlobalExceptionHandler
{
    public static async Task HandleExceptionAsync(Exception ex)
    {
        var message = ex switch
        {
            DatabaseConnectionException => "データベースに接続できません。ネットワーク接続を確認してください。",
            BarcodeScanException => "バーコードの読み取りに失敗しました。再度お試しください。",
            InsufficientStockException stockEx => $"在庫が不足しています。利用可能数量: {stockEx.AvailableStock}",
            _ => "予期しないエラーが発生しました。"
        };
        
        await Application.Current.MainPage.DisplayAlert("エラー", message, "OK");
    }
}
```

## テスト戦略

### 1. 単体テスト

#### Service Layer テスト
```csharp
[TestClass]
public class StockOutServiceTests
{
    private Mock<IItemRepository> _itemRepositoryMock;
    private Mock<ITransactionRepository> _transactionRepositoryMock;
    private StockOutService _service;
    
    [TestInitialize]
    public void Setup()
    {
        _itemRepositoryMock = new Mock<IItemRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _service = new StockOutService(_itemRepositoryMock.Object, _transactionRepositoryMock.Object);
    }
    
    [TestMethod]
    public async Task ProcessStockOut_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new StockOutRequest { ItemId = 1, Quantity = 5, DepartmentId = 1, ProcessedBy = "Test" };
        var item = new Item { ItemId = 1, CurrentStock = 10 };
        
        _itemRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(item);
        
        // Act
        var result = await _service.ProcessStockOutAsync(request);
        
        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(5, item.CurrentStock);
    }
}
```

#### Repository Layer テスト
```csharp
[TestClass]
public class ItemRepositoryTests
{
    private OfficeSuppliesContext _context;
    private ItemRepository _repository;
    
    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OfficeSuppliesContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new OfficeSuppliesContext(options);
        _repository = new ItemRepository(_context);
    }
    
    [TestMethod]
    public async Task GetByBarcode_ExistingBarcode_ReturnsItem()
    {
        // Arrange
        var item = new Item { ItemCode = "TEST001", ItemName = "Test Item", IsActive = true };
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _repository.GetByBarcodeAsync("TEST001");
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TEST001", result.ItemCode);
    }
}
```

### 2. 統合テスト

#### データベース統合テスト
```csharp
[TestClass]
public class DatabaseIntegrationTests
{
    private OfficeSuppliesContext _context;
    
    [TestInitialize]
    public void Setup()
    {
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=OfficeSuppliesTest;Trusted_Connection=true;";
        var options = new DbContextOptionsBuilder<OfficeSuppliesContext>()
            .UseSqlServer(connectionString)
            .Options;
            
        _context = new OfficeSuppliesContext(options);
        _context.Database.EnsureCreated();
    }
    
    [TestMethod]
    public async Task StockOutWorkflow_EndToEnd_Success()
    {
        // 完全なワークフローテスト
        // 1. 部署取得
        // 2. アイテム検索
        // 3. 在庫更新
        // 4. 取引記録
    }
}
```

### 3. UIテスト

#### ViewModel テスト
```csharp
[TestClass]
public class DepartmentSelectionViewModelTests
{
    private Mock<IDepartmentService> _departmentServiceMock;
    private DepartmentSelectionViewModel _viewModel;
    
    [TestInitialize]
    public void Setup()
    {
        _departmentServiceMock = new Mock<IDepartmentService>();
        _viewModel = new DepartmentSelectionViewModel(_departmentServiceMock.Object);
    }
    
    [TestMethod]
    public async Task LoadDepartments_Success_PopulatesList()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department { DepartmentId = 1, DepartmentName = "総務部" }
        };
        _departmentServiceMock.Setup(x => x.GetActiveDepartmentsAsync()).ReturnsAsync(departments);
        
        // Act
        await _viewModel.LoadDepartmentsCommand.ExecuteAsync(null);
        
        // Assert
        Assert.AreEqual(1, _viewModel.Departments.Count);
    }
}
```

## プラットフォーム固有実装

### Android固有実装

#### カメラ権限
```csharp
// Platforms/Android/MainApplication.cs
[assembly: UsesPermission(Android.Manifest.Permission.Camera)]
[assembly: UsesPermission(Android.Manifest.Permission.Flashlight)]
```

#### バーコードスキャン実装
```csharp
// Platforms/Android/Services/AndroidBarcodeService.cs
public class AndroidBarcodeService : IBarcodeService
{
    public async Task<string> ScanBarcodeAsync()
    {
        var scanner = new ZXingScannerView(Platform.CurrentActivity);
        // ZXing.Net.Mobile実装
        return await scanner.ScanAsync();
    }
}
```

### 依存性注入設定

#### MauiProgram.cs
```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Database
        builder.Services.AddDbContext<OfficeSuppliesContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Services
        builder.Services.AddScoped<IDepartmentService, DepartmentService>();
        builder.Services.AddScoped<IItemService, ItemService>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddScoped<IStockOutService, StockOutService>();

        // Platform-specific services
#if ANDROID
        builder.Services.AddSingleton<IBarcodeService, AndroidBarcodeService>();
#elif IOS
        builder.Services.AddSingleton<IBarcodeService, iOSBarcodeService>();
#else
        builder.Services.AddSingleton<IBarcodeService, MockBarcodeService>();
#endif

        // ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<DepartmentSelectionViewModel>();
        builder.Services.AddTransient<BarcodeScannerViewModel>();
        builder.Services.AddTransient<QuantityInputViewModel>();
        builder.Services.AddTransient<ConfirmationViewModel>();
        builder.Services.AddTransient<ResultViewModel>();

        return builder.Build();
    }
}
```

この設計により、既存のEntity Frameworkモデルを活用しながら、保守性と拡張性に優れた.NET MAUIアプリケーションを構築できます。