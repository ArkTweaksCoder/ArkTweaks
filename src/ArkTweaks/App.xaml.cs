using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Core.Safety;
using ArkTweaks.UI.Navigation;

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

        // Navigation
        services.AddSingleton<NavigationService>();

        // System Services (active features only)
        services.AddSingleton<SystemInfoService>();
        services.AddSingleton<TempCleanerService>();
        services.AddSingleton<StartupService>();
        services.AddSingleton<RestorePointService>();
        services.AddSingleton<PowerPlanService>();

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