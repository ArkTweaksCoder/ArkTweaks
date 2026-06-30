using System;

namespace ArkTweaks.Models;

/// <summary>
/// Comprehensive system scan result containing all gathered system information.
/// </summary>
public class SystemScanResult
{
    /// <summary>
    /// Windows version string (e.g., "Microsoft Windows NT 10.0.19045")
    /// </summary>
    public string WindowsVersion { get; set; } = string.Empty;

    /// <summary>
    /// CPU processor name
    /// </summary>
    public string CpuName { get; set; } = "Unknown";

    /// <summary>
    /// GPU name (primary display adapter)
    /// </summary>
    public string GpuName { get; set; } = "Unknown";

    /// <summary>
    /// Total installed RAM in GB
    /// </summary>
    public double RamInstalledGB { get; set; }

    /// <summary>
    /// Current RAM usage percentage (0-100)
    /// </summary>
    public double RamUsagePercent { get; set; }

    /// <summary>
    /// Disk usage percentage (0-100)
    /// </summary>
    public double DiskUsagePercent { get; set; }

    /// <summary>
    /// Free disk space in GB
    /// </summary>
    public double FreeDiskSpaceGB { get; set; }

    /// <summary>
    /// Current power plan name
    /// </summary>
    public string CurrentPowerPlan { get; set; } = "Unknown";

    /// <summary>
    /// Number of startup applications
    /// </summary>
    public int StartupAppCount { get; set; }

    /// <summary>
    /// Windows uptime in hours
    /// </summary>
    public double WindowsUptimeHours { get; set; }

    /// <summary>
    /// Whether a restore point exists
    /// </summary>
    public bool HasRestorePoint { get; set; }

    /// <summary>
    /// Whether Storage Sense is enabled
    /// </summary>
    public bool StorageSenseEnabled { get; set; }

    /// <summary>
    /// Whether Game Mode is enabled
    /// </summary>
    public bool GameModeEnabled { get; set; }

    /// <summary>
    /// Whether Hardware Accelerated GPU Scheduling is enabled
    /// </summary>
    public bool HAGpuSchedulingEnabled { get; set; }

    /// <summary>
    /// Temporary files size in MB
    /// </summary>
    public double TempFilesSizeMB { get; set; }

    /// <summary>
    /// Timestamp when scan was performed
    /// </summary>
    public DateTime ScanTimestamp { get; set; } = DateTime.UtcNow;
}
