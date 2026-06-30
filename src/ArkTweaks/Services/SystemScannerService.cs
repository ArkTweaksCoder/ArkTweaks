using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ArkTweaks.Models;

namespace ArkTweaks.Services;

/// <summary>
/// Service for scanning and gathering comprehensive system information.
/// </summary>
public class SystemScannerService
{
    private readonly ILogger<SystemScannerService> _logger;
    private readonly SystemInfoService _systemInfoService;
    private readonly StartupService _startupService;
    private readonly PowerPlanService _powerPlanService;
    private readonly TempCleanerService _tempCleanerService;

    public SystemScannerService(
        ILogger<SystemScannerService> logger,
        SystemInfoService systemInfoService,
        StartupService startupService,
        PowerPlanService powerPlanService,
        TempCleanerService tempCleanerService)
    {
        _logger = logger;
        _systemInfoService = systemInfoService;
        _startupService = startupService;
        _powerPlanService = powerPlanService;
        _tempCleanerService = tempCleanerService;
    }

    /// <summary>
    /// Performs a comprehensive system scan.
    /// </summary>
    public SystemScanResult ScanSystem()
    {
        var result = new SystemScanResult();

        try
        {
            // Windows Version
            result.WindowsVersion = Environment.OSVersion.VersionString;

            // CPU Name
            result.CpuName = GetCpuName();

            // GPU Name
            result.GpuName = GetGpuName();

            // RAM Information
            result.RamInstalledGB = GetTotalRamGB();
            var systemInfo = _systemInfoService.GetSystemInfo();
            result.RamUsagePercent = systemInfo.RamUsage;

            // Disk Information
            result.DiskUsagePercent = systemInfo.StorageUsage;
            result.FreeDiskSpaceGB = GetFreeDiskSpaceGB();

            // Power Plan
            var powerPlans = _powerPlanService.GetPowerPlans();
            var activePlan = powerPlans.FirstOrDefault(p => p.IsActive);
            result.CurrentPowerPlan = activePlan?.Name ?? "Unknown";

            // Startup Apps
            var startupApps = _startupService.GetStartupItems();
            result.StartupAppCount = startupApps.Count;

            // Windows Uptime
            result.WindowsUptimeHours = GetWindowsUptimeHours();

            // Restore Point Status
            result.HasRestorePoint = CheckRestorePointExists();

            // Storage Sense Status
            result.StorageSenseEnabled = CheckStorageSenseEnabled();

            // Game Mode Status
            result.GameModeEnabled = CheckGameModeEnabled();

            // Hardware Accelerated GPU Scheduling
            result.HAGpuSchedulingEnabled = CheckHAGpuSchedulingEnabled();

            // Temporary Files Size
            result.TempFilesSizeMB = _tempCleanerService.ScanTempFiles() / 1024.0 / 1024.0;

            _logger.LogInformation("System scan completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during system scan");
        }

        return result;
    }

    private string GetCpuName()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["Name"]?.ToString() ?? "Unknown";
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get CPU name");
        }
        return "Unknown";
    }

    private string GetGpuName()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["Name"]?.ToString() ?? "Unknown";
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get GPU name");
        }
        return "Unknown";
    }

    private double GetTotalRamGB()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                var bytes = Convert.ToDouble(obj["TotalPhysicalMemory"]);
                return bytes / 1024.0 / 1024.0 / 1024.0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get total RAM");
        }
        return 0;
    }

    private double GetFreeDiskSpaceGB()
    {
        try
        {
            var systemDrive = Path.GetPathRoot(Environment.SystemDirectory);
            if (string.IsNullOrEmpty(systemDrive))
            {
                return 0;
            }
            var drive = new DriveInfo(systemDrive);
            return drive.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get free disk space");
        }
        return 0;
    }

    private double GetWindowsUptimeHours()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                var bootTime = ManagementDateTimeConverter.ToDateTime(obj["LastBootUpTime"].ToString());
                var uptime = DateTime.Now - bootTime;
                return uptime.TotalHours;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get Windows uptime");
        }
        return 0;
    }

    private bool CheckRestorePointExists()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SystemRestore");
            return searcher.Get().Count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check restore point status");
        }
        return false;
    }

    private bool CheckStorageSenseEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy");
            if (key != null)
            {
                var value = key.GetValue("01");
                return value != null && Convert.ToInt32(value) == 1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check Storage Sense status");
        }
        return false;
    }

    private bool CheckGameModeEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\GameBar");
            if (key != null)
            {
                var value = key.GetValue("AllowAutoGameMode");
                return value != null && Convert.ToInt32(value) == 1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check Game Mode status");
        }
        return false;
    }

    private bool CheckHAGpuSchedulingEnabled()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers");
            if (key != null)
            {
                var value = key.GetValue("HwSchMode");
                return value != null && Convert.ToInt32(value) == 2;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check HA GPU Scheduling status");
        }
        return false;
    }
}
