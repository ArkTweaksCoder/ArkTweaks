using System;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly SystemInfoService _systemInfoService;
    private string _cpuUsage = "0%";
    private string _ramUsage = "0%";
    private string _storageUsage = "0%";
    private string _optimizationScore = "85";

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

    public DashboardViewModel(ILogger<DashboardViewModel> logger, SystemInfoService systemInfoService) 
        : base(logger)
    {
        _systemInfoService = systemInfoService;
        LoadSystemInfo();
    }

    private void LoadSystemInfo()
    {
        try
        {
            var info = _systemInfoService.GetSystemInfo();
            CpuUsage = $"{info.CpuUsage}%";
            RamUsage = $"{info.RamUsage}%";
            StorageUsage = $"{info.StorageUsage}%";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load system info");
        }
    }
}
