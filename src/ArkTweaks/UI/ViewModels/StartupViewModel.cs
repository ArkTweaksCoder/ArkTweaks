using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Core.Engine;

namespace ArkTweaks.UI.ViewModels;

public class StartupViewModel : BaseViewModel
{
    private readonly StartupService _startupService;
    private readonly LicenseService _licenseService;
    private readonly OptimizationEngine _optimizationEngine;
    private bool _isLoading;

    public ObservableCollection<StartupItemViewModel> StartupApps { get; } = new();

    public bool CanManage => _licenseService.IsFeatureEnabled(LicenseFeature.StartupManager);

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public StartupViewModel(
        ILogger<StartupViewModel> logger,
        StartupService startupService,
        LicenseService licenseService,
        OptimizationEngine optimizationEngine) 
        : base(logger)
    {
        _startupService = startupService;
        _licenseService = licenseService;
        _optimizationEngine = optimizationEngine;
        LoadStartupApps();
    }

    public async void LoadStartupApps()
    {
        IsLoading = true;
        try
        {
            var apps = await _startupService.GetStartupAppsAsync();
            StartupApps.Clear();
            
            foreach (var app in apps)
            {
                StartupApps.Add(new StartupItemViewModel
                {
                    Name = app.Name,
                    Path = app.Path,
                    IsEnabled = app.Enabled
                });
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load startup apps");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task ToggleStartupAppAsync(StartupItemViewModel app)
    {
        try
        {
            if (app.IsEnabled)
            {
                // Disable
                var result = await _startupService.DisableStartupAppAsync(app.Name, app.Path);
                if (result)
                {
                    app.IsEnabled = false;
                }
            }
            else
            {
                // Enable
                var result = await _startupService.EnableStartupAppAsync(app.Name, app.Path);
                if (result)
                {
                    app.IsEnabled = true;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to toggle startup app: {AppName}", app.Name);
        }
    }

    public void Refresh()
    {
        LoadStartupApps();
    }
}

public class StartupItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
