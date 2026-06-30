using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Core.Safety;
using ArkTweaks.UI.Navigation;
using ArkTweaks.UI.ViewModels;

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

        // Create and show main window
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(configure => configure.AddConsole());

        // Core Services
        services.AddSingleton<LicenseService>();
        services.AddSingleton<SafetyValidator>();

        // Navigation
        services.AddSingleton<NavigationService>();

        // System Services (active features only)
        services.AddSingleton<SystemInfoService>();
        services.AddSingleton<TempCleanerService>();
        services.AddSingleton<StartupService>();
        services.AddSingleton<RestorePointService>();
        services.AddSingleton<PowerPlanService>();

        // ViewModels
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<OptimizeViewModel>();
        services.AddTransient<CleanupViewModel>();
        services.AddTransient<StartupViewModel>();
        services.AddTransient<GamingViewModel>();
        services.AddTransient<PerformanceViewModel>();
        services.AddTransient<RestoreViewModel>();
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