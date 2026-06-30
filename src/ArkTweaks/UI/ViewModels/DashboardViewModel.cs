using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly SystemInfoService _systemInfoService;
    private readonly StartupService _startupService;
    private readonly PowerPlanService _powerPlanService;
    private string _cpuUsage = "0%";
    private string _ramUsage = "0%";
    private string _storageUsage = "0%";
    private string _optimizationScore = "85";
    private string _windowsVersion = "Unknown";
    private string _currentPowerPlan = "Balanced";
    private int _startupAppCount = 0;

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

    public DashboardViewModel(
        ILogger<DashboardViewModel> logger,
        SystemInfoService systemInfoService,
        StartupService startupService,
        PowerPlanService powerPlanService) 
        : base(logger)
    {
        _systemInfoService = systemInfoService;
        _startupService = startupService;
        _powerPlanService = powerPlanService;
        LoadSystemInfo();
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

    private async void StartPeriodicRefresh()
    {
        while (true)
        {
            await Task.Delay(5000); // Refresh every 5 seconds
            LoadSystemInfo();
        }
    }
}
