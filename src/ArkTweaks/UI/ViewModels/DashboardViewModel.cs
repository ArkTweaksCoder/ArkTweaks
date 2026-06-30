using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Core;
using ArkTweaks.Core.Engine;
using ArkTweaks.Core.Logging;
using ArkTweaks.Models;

namespace ArkTweaks.UI.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly SystemInfoService _systemInfoService;
    private readonly StartupService _startupService;
    private readonly PowerPlanService _powerPlanService;
    private readonly SystemScannerService _systemScannerService;
    private readonly RecommendationEngine _recommendationEngine;
    private readonly HealthScoreService _healthScoreService;
    private readonly OptimizationEngine _optimizationEngine;
    private readonly ActionLogger _actionLogger;
    private string _cpuUsage = "0%";
    private string _ramUsage = "0%";
    private string _storageUsage = "0%";
    private string _optimizationScore = "85";
    private string _windowsVersion = "Unknown";
    private string _currentPowerPlan = "Balanced";
    private int _startupAppCount = 0;
    private bool _isScanning;
    private HealthScore _healthScore = new();
    private SystemScanResult _scanResult = new();

    // Modern Card Data
    private string _cpuName = "Unknown";
    private string _gpuName = "Unknown";
    private double _ramInstalledGB = 0;
    private double _freeDiskSpaceGB = 0;
    private double _windowsUptimeHours = 0;

    // New UI Properties
    private string _healthScoreDisplay = "85";
    private string _healthStatus = "Good";
    private string _diskUsage = "58%";
    private string _healthTrend = "+5%";
    private string _cleanupTrend = "+12%";
    private string _storageTrend = "-2%";

    public string CpuUsage
    {
        get => _cpuUsage;
        set => SetProperty(ref _cpuUsage, value);
    }

    public string RamUsage
    {
        get => _ramUsage;
        set => SetProperty(ref _ramUsage, value);
    }

    public string StorageUsage
    {
        get => _storageUsage;
        set => SetProperty(ref _storageUsage, value);
    }

    public string OptimizationScore
    {
        get => _optimizationScore;
        set => SetProperty(ref _optimizationScore, value);
    }

    public string WindowsVersion
    {
        get => _windowsVersion;
        set => SetProperty(ref _windowsVersion, value);
    }

    public string CurrentPowerPlan
    {
        get => _currentPowerPlan;
        set => SetProperty(ref _currentPowerPlan, value);
    }

    public int StartupAppCount
    {
        get => _startupAppCount;
        set => SetProperty(ref _startupAppCount, value);
    }

    public bool IsScanning
    {
        get => _isScanning;
        set => SetProperty(ref _isScanning, value);
    }

    public HealthScore HealthScore
    {
        get => _healthScore;
        set => SetProperty(ref _healthScore, value);
    }

    // Modern Card Properties
    public string CpuName
    {
        get => _cpuName;
        set => SetProperty(ref _cpuName, value);
    }

    public string GpuName
    {
        get => _gpuName;
        set => SetProperty(ref _gpuName, value);
    }

    public double RamInstalledGB
    {
        get => _ramInstalledGB;
        set => SetProperty(ref _ramInstalledGB, value);
    }

    public double FreeDiskSpaceGB
    {
        get => _freeDiskSpaceGB;
        set => SetProperty(ref _freeDiskSpaceGB, value);
    }

    public double WindowsUptimeHours
    {
        get => _windowsUptimeHours;
        set => SetProperty(ref _windowsUptimeHours, value);
    }

    // New UI Properties
    public string HealthScoreDisplay
    {
        get => _healthScoreDisplay;
        set => SetProperty(ref _healthScoreDisplay, value);
    }

    public string HealthStatus
    {
        get => _healthStatus;
        set => SetProperty(ref _healthStatus, value);
    }

    public string DiskUsage
    {
        get => _diskUsage;
        set => SetProperty(ref _diskUsage, value);
    }

    public string HealthTrend
    {
        get => _healthTrend;
        set => SetProperty(ref _healthTrend, value);
    }

    public string CleanupTrend
    {
        get => _cleanupTrend;
        set => SetProperty(ref _cleanupTrend, value);
    }

    public string StorageTrend
    {
        get => _storageTrend;
        set => SetProperty(ref _storageTrend, value);
    }

    // Collections
    public ObservableCollection<RecentActivityItem> RecentActivities { get; } = new();
    public ObservableCollection<Recommendation> Recommendations { get; } = new();

    // Commands
    public ICommand? OptimizeCommand { get; }
    public ICommand? CleanupCommand { get; }
    public ICommand? RefreshCommand { get; }
    public ICommand? RestorePointCommand { get; }

    public DashboardViewModel(
        ILogger<DashboardViewModel> logger,
        SystemInfoService systemInfoService,
        StartupService startupService,
        PowerPlanService powerPlanService,
        SystemScannerService systemScannerService,
        RecommendationEngine recommendationEngine,
        HealthScoreService healthScoreService,
        OptimizationEngine optimizationEngine,
        ActionLogger actionLogger) 
        : base(logger)
    {
        _systemInfoService = systemInfoService;
        _startupService = startupService;
        _powerPlanService = powerPlanService;
        _systemScannerService = systemScannerService;
        _recommendationEngine = recommendationEngine;
        _healthScoreService = healthScoreService;
        _optimizationEngine = optimizationEngine;
        _actionLogger = actionLogger;
        
        // Initialize commands
        OptimizeCommand = new RelayCommand(async () => await QuickOptimizeAsync());
        CleanupCommand = new RelayCommand(async () => await QuickCleanAsync());
        RefreshCommand = new RelayCommand(async () => await PerformFullScanAsync());
        RestorePointCommand = new RelayCommand(async () => await CreateRestorePointAsync());
        
        LoadSystemInfo();
        LoadRecentActivities();
        StartPeriodicRefresh();
    }

    private void LoadSystemInfo()
    {
        try
        {
            var info = _systemInfoService.GetSystemInfo();
            CpuUsage = $"{info.CpuUsage}%";
            RamUsage = $"{info.RamUsage}%";
            StorageUsage = $"{info.StorageUsage}%";
            WindowsVersion = Environment.OSVersion.VersionString;
            
            var startupApps = _startupService.GetStartupItems();
            StartupAppCount = startupApps.Count;
            
            var powerPlans = _powerPlanService.GetPowerPlans();
            var activePlan = powerPlans.FirstOrDefault(p => p.IsActive);
            CurrentPowerPlan = activePlan?.Name ?? "Unknown";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load system info");
        }
    }

    private void LoadRecentActivities()
    {
        try
        {
            // Load recent activities from action logger
            // This is a simplified implementation
            RecentActivities.Clear();
            
            // Add sample data for now
            RecentActivities.Add(new RecentActivityItem
            {
                Timestamp = DateTime.Now.AddMinutes(-5),
                Action = "System Scan",
                Success = true
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load recent activities");
        }
    }

    public Task PerformFullScanAsync()
    {
        return Task.Run(() =>
        {
            IsScanning = true;
            try
            {
                _scanResult = _systemScannerService.ScanSystem();
                
                // Update modern card data
                CpuName = _scanResult.CpuName;
                GpuName = _scanResult.GpuName;
                RamInstalledGB = _scanResult.RamInstalledGB;
                FreeDiskSpaceGB = _scanResult.FreeDiskSpaceGB;
                WindowsUptimeHours = _scanResult.WindowsUptimeHours;
                
                // Calculate health score
                HealthScore = _healthScoreService.CalculateHealthScore(_scanResult);
                OptimizationScore = HealthScore.OverallScore.ToString();
                
                // Generate recommendations
                var recommendations = _recommendationEngine.GenerateRecommendations(_scanResult);
                Recommendations.Clear();
                foreach (var rec in recommendations)
                {
                    Recommendations.Add(rec);
                }
                
                Logger.LogInformation("Full scan completed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during full scan");
            }
            finally
            {
                IsScanning = false;
            }
        });
    }

    public async Task QuickOptimizeAsync()
    {
        try
        {
            // Run safe optimizations
            await _optimizationEngine.ExecuteTweakAsync("cleanup_temp_files");
            await _optimizationEngine.ExecuteTweakAsync("restore_create_point");
            
            Logger.LogInformation("Quick optimization completed");
            await PerformFullScanAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during quick optimization");
        }
    }

    public async Task QuickCleanAsync()
    {
        try
        {
            await _optimizationEngine.ExecuteTweakAsync("cleanup_temp_files");
            await _optimizationEngine.ExecuteTweakAsync("cleanup_recycle_bin");
            
            Logger.LogInformation("Quick clean completed");
            await PerformFullScanAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during quick clean");
        }
    }

    public async Task CreateRestorePointAsync()
    {
        try
        {
            await _optimizationEngine.ExecuteTweakAsync("restore_create_point");
            Logger.LogInformation("Restore point created");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating restore point");
        }
    }

    public async Task ApplyRecommendationAsync(Recommendation recommendation)
    {
        try
        {
            var result = await _optimizationEngine.ExecuteTweakAsync(recommendation.SuggestedAction);
            if (result.Success)
            {
                recommendation.IsApplied = true;
                Logger.LogInformation("Applied recommendation: {Title}", recommendation.Title);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying recommendation: {Title}", recommendation.Title);
        }
    }

    private async void StartPeriodicRefresh()
    {
        while (true)
        {
            await Task.Delay(30000); // Refresh every 30 seconds
            LoadSystemInfo();
        }
    }
}

public class RecentActivityItem
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public bool Success { get; set; }
}
