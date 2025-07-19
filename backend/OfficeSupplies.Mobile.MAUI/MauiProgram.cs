using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OfficeSupplies.Infrastructure.Data;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;
using OfficeSupplies.Mobile.MAUI.Repositories.Implementations;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;
using OfficeSupplies.Mobile.MAUI.Services.Implementations;
using ZXing.Net.Maui.Controls;
using CommunityToolkit.Maui;

namespace OfficeSupplies.Mobile.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Database
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=OfficeSupplies;Trusted_Connection=true;";
        builder.Services.AddDbContext<OfficeSuppliesContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Services 
        builder.Services.AddScoped<IDepartmentService, DepartmentService>();
        builder.Services.AddScoped<IItemService, ItemService>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddScoped<IStockOutService, StockOutService>();

        // ViewModels
        // (will be added later)

        // Platform-specific services
#if ANDROID
        // Android specific services will be added here
#elif IOS
        // iOS specific services will be added here
#elif WINDOWS
        // Windows specific services will be added here
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}