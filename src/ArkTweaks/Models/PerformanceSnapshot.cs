using System;

namespace ArkTweaks.Models;

/// <summary>
/// Represents a snapshot of system performance at a point in time.
/// </summary>
public class PerformanceSnapshot
{
    /// <summary>
    /// Timestamp when the snapshot was taken
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// CPU usage percentage (0-100)
    /// </summary>
    public double CpuUsagePercent { get; set; }

    /// <summary>
    /// RAM usage percentage (0-100)
    /// </summary>
    public double RamUsagePercent { get; set; }

    /// <summary>
    /// Disk usage percentage (0-100)
    /// </summary>
    public double DiskUsagePercent { get; set; }

    /// <summary>
    /// Available storage space in GB
    /// </summary>
    public double StorageAvailableGB { get; set; }

    /// <summary>
    /// Overall health score (0-100)
    /// </summary>
    public int HealthScore { get; set; }

    /// <summary>
    /// Number of startup applications
    /// </summary>
    public int StartupAppCount { get; set; }

    /// <summary>
    /// Current power plan name
    /// </summary>
    public string CurrentPowerPlan { get; set; } = string.Empty;

    /// <summary>
    /// Windows uptime in hours
    /// </summary>
    public double WindowsUptimeHours { get; set; }
}
