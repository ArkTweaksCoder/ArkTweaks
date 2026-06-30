using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Core;
using ArkTweaks.Core.Safety;
using ArkTweaks.Core.Logging;
using ArkTweaks.Core.Restore;
using ArkTweaks.Core.Engine;
using ArkTweaks.Core.Tweaks;
using ArkTweaks.Core.Registry;
using ArkTweaks.UI.Navigation;
using ArkTweaks.UI.ViewModels;
using ArkTweaks.Tweaks.Storage;
using ArkTweaks.Tweaks.Power;
using ArkTweaks.Tweaks.Restore;
using ArkTweaks.Tweaks.Startup;

namespace ArkTweaks;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }
    public static ILogger<T> GetLogger<T>() => ServiceProvider!.GetRequiredService<ILogger<T>>();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Initialize license service
            var licenseService = ServiceProvider.GetRequiredService<LicenseService>();
            licenseService.Initialize();

            // Initialize optimization engine with tweaks
            var optimizationEngine = ServiceProvider.GetRequiredService<OptimizationEngine>();
            optimizationEngine.RegisterTweak(ServiceProvider.GetRequiredService<TemporaryFilesCleanupTweak>());
            optimizationEngine.RegisterTweak(ServiceProvider.GetRequiredService<RecycleBinCleanupTweak>());
            optimizationEngine.RegisterTweak(ServiceProvider.GetRequiredService<HighPerformancePowerPlanTweak>());
            optimizationEngine.RegisterTweak(ServiceProvider.GetRequiredService<CreateRestorePointTweak>());

            // Create and show main window
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting application: {ex.Message}\n\n{ex.StackTrace}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(configure => configure.AddConsole());

        // Core Services
        services.AddSingleton<LicenseService>();
        services.AddSingleton<SafetyValidator>();
        services.AddSingleton<ActionLogger>();
        services.AddSingleton<BackupService>();
        services.AddSingleton<OptimizationEngine>();

        // Registry Framework Services
        services.AddSingleton<RegistryService>();
        services.AddSingleton<RegistryBackupService>();
        services.AddSingleton<RegistryRestoreService>();

        // System Scanner & Health Services
        services.AddSingleton<SystemScannerService>();
        services.AddSingleton<RecommendationEngine>();
        services.AddSingleton<HealthScoreService>();

        // Cleanup Services
        services.AddSingleton<CleanupScannerService>();
        services.AddSingleton<CleanupHistoryService>();

        // Performance History Services
        services.AddSingleton<PerformanceHistoryService>();

        // Recommendation Services
        services.AddSingleton<RecommendationExecutionService>();
        services.AddSingleton<RecommendationHistoryService>();

        // Settings Service
        services.AddSingleton<SettingsService>();

        // Navigation
        services.AddSingleton<NavigationService>();

        // System Services (active features only)
        services.AddSingleton<SystemInfoService>();
        services.AddSingleton<TempCleanerService>();
        services.AddSingleton<StartupService>();
        services.AddSingleton<RestorePointService>();
        services.AddSingleton<PowerPlanService>();

        // Tweaks
        services.AddSingleton<TemporaryFilesCleanupTweak>();
        services.AddSingleton<RecycleBinCleanupTweak>();
        services.AddSingleton<HighPerformancePowerPlanTweak>();
        services.AddSingleton<CreateRestorePointTweak>();

        // ViewModels
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<OptimizeViewModel>();
        services.AddTransient<CleanupViewModel>();
        services.AddTransient<StartupViewModel>();
        services.AddTransient<GamingViewModel>();
        services.AddTransient<PerformanceViewModel>();
        services.AddTransient<RestoreViewModel>();
        services.AddTransient<HistoryViewModel>();
        services.AddTransient<LogsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<AboutViewModel>();

        // Main Window
        services.AddTransient<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        base.OnExit(e);
    }
}