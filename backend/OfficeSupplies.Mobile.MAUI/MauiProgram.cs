using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using OfficeSupplies.Infrastructure.Data;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;
using OfficeSupplies.Mobile.MAUI.Repositories.Implementations;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;
using OfficeSupplies.Mobile.MAUI.Services.Implementations;
using ZXing.Net.Maui.Controls;
using CommunityToolkit.Maui;
using System.Reflection;

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

        // Configuration
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json");
        
        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream!)
            .Build();
        
        builder.Configuration.AddConfiguration(configuration);

        // Database
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
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
#else
        // Mock service for testing and development
        builder.Services.AddSingleton<IBarcodeService, MockBarcodeService>();
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}