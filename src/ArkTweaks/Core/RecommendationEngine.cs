using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ArkTweaks.Models;
using ArkTweaks.Core.Safety;

namespace ArkTweaks.Core;

/// <summary>
/// Engine for analyzing system scan results and generating optimization recommendations.
/// </summary>
public class RecommendationEngine
{
    private readonly ILogger<RecommendationEngine> _logger;
    private readonly HashSet<string> _implementedTweaks = new()
    {
        "cleanup_temp_files",
        "cleanup_recycle_bin",
        "power_high_performance",
        "restore_create_point",
        "manage_startup_apps"
    };

    public RecommendationEngine(ILogger<RecommendationEngine> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates recommendations based on system scan results with contextual analysis.
    /// </summary>
    public List<Recommendation> GenerateRecommendations(SystemScanResult scanResult)
    {
        var recommendations = new List<Recommendation>();

        try
        {
            // Check for large temp folder
            if (scanResult.TempFilesSizeMB > 500)
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_large_temp",
                    Title = "Large Temporary Files Folder",
                    Description = "Your temporary files folder contains over 500MB of data that can be safely cleaned.",
                    DetailedExplanation = "Temporary files accumulate over time from various applications and Windows operations. Cleaning these files can free up significant disk space and may improve system performance by reducing file system overhead.",
                    Reason = $"Found {scanResult.TempFilesSizeMB:F0}MB of temporary files",
                    EstimatedImpact = "Medium",
                    Difficulty = "Easy",
                    RiskLevel = RiskLevel.Low,
                    EstimatedCompletionTimeSeconds = 30,
                    SuggestedAction = "cleanup_temp_files",
                    Category = "Cleanup",
                    Priority = RecommendationPriority.High,
                    RequiredLicenseTier = LicenseTier.Free,
                    SystemArea = "Storage",
                    HasImplementation = _implementedTweaks.Contains("cleanup_temp_files")
                });
            }

            // Check for too many startup apps
            if (scanResult.StartupAppCount > 10)
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_many_startup",
                    Title = "Too Many Startup Applications",
                    Description = "Having many startup applications can slow down boot time and overall system performance.",
                    DetailedExplanation = "Each startup application consumes system resources and increases boot time. Reducing the number of startup applications can significantly improve boot speed and free up RAM and CPU resources for active tasks.",
                    Reason = $"{scanResult.StartupAppCount} applications are set to launch at startup",
                    EstimatedImpact = "High",
                    Difficulty = "Medium",
                    RiskLevel = RiskLevel.Low,
                    EstimatedCompletionTimeSeconds = 60,
                    SuggestedAction = "manage_startup_apps",
                    Category = "Startup",
                    Priority = RecommendationPriority.High,
                    RequiredLicenseTier = LicenseTier.Free,
                    SystemArea = "Performance",
                    HasImplementation = _implementedTweaks.Contains("manage_startup_apps")
                });
            }

            // Check if balanced power plan is active
            if (scanResult.CurrentPowerPlan == "Balanced")
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_power_plan",
                    Title = "Switch to High Performance Power Plan",
                    Description = "The High Performance power plan can improve system responsiveness and gaming performance.",
                    DetailedExplanation = "The High Performance power plan prevents the CPU from throttling and maintains maximum clock speeds. This can improve performance in CPU-intensive tasks and gaming, though it may increase power consumption.",
                    Reason = "Currently using Balanced power plan",
                    EstimatedImpact = "Medium",
                    Difficulty = "Easy",
                    RiskLevel = RiskLevel.Low,
                    EstimatedCompletionTimeSeconds = 10,
                    SuggestedAction = "power_high_performance",
                    Category = "Power",
                    Priority = RecommendationPriority.Medium,
                    RequiredLicenseTier = LicenseTier.Free,
                    SystemArea = "Power",
                    HasImplementation = _implementedTweaks.Contains("power_high_performance")
                });
            }

            // Check if storage is nearly full
            if (scanResult.DiskUsagePercent > 90)
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_storage_full",
                    Title = "Storage Nearly Full",
                    Description = "Your disk is over 90% full. This can significantly impact system performance and cause issues.",
                    DetailedExplanation = "When disk space is critically low, Windows cannot create temporary files, perform updates, or maintain system health. This can lead to system instability, slow performance, and inability to save files.",
                    Reason = $"Disk usage is at {scanResult.DiskUsagePercent:F0}%",
                    EstimatedImpact = "High",
                    Difficulty = "Medium",
                    RiskLevel = RiskLevel.Low,
                    EstimatedCompletionTimeSeconds = 120,
                    SuggestedAction = "cleanup_storage",
                    Category = "Storage",
                    Priority = RecommendationPriority.Critical,
                    RequiredLicenseTier = LicenseTier.Free,
                    SystemArea = "Storage",
                    HasImplementation = false
                });
            }

            // Check if Game Mode is disabled
            if (!scanResult.GameModeEnabled)
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_game_mode",
                    Title = "Enable Game Mode",
                    Description = "Windows Game Mode can improve gaming performance by prioritizing games and background processes.",
                    DetailedExplanation = "Game Mode optimizes your PC for gaming by preventing Windows Update from installing drivers or showing restart notifications, and helps achieve a more consistent gaming experience by prioritizing games.",
                    Reason = "Game Mode is currently disabled",
                    EstimatedImpact = "Medium",
                    Difficulty = "Easy",
                    RiskLevel = RiskLevel.Low,
                    EstimatedCompletionTimeSeconds = 15,
                    SuggestedAction = "enable_game_mode",
                    Category = "Gaming",
                    Priority = RecommendationPriority.Medium,
                    RequiredLicenseTier = LicenseTier.Free,
                    SystemArea = "Gaming",
                    HasImplementation = false
                });
            }

            // Check if Storage Sense is disabled
            if (!scanResult.StorageSenseEnabled)
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_storage_sense",
                    Title = "Enable Storage Sense",
                    Description = "Storage Sense automatically cleans up temporary files to maintain disk space.",
                    DetailedExplanation = "Storage Sense automatically frees up space by deleting temporary files and content in your recycle bin. It can help maintain optimal disk space without manual intervention.",
                    Reason = "Storage Sense is currently disabled",
                    EstimatedImpact = "Medium",
                    Difficulty = "Easy",
                    RiskLevel = RiskLevel.Low,
                    EstimatedCompletionTimeSeconds = 20,
                    SuggestedAction = "enable_storage_sense",
                    Category = "Storage",
                    Priority = RecommendationPriority.Medium,
                    RequiredLicenseTier = LicenseTier.Free,
                    SystemArea = "Maintenance",
                    HasImplementation = false
                });
            }

            // Check if no restore point exists
            if (!scanResult.HasRestorePoint)
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_restore_point",
                    Title = "Create System Restore Point",
                    Description = "A restore point allows you to revert system changes if something goes wrong.",
                    DetailedExplanation = "System restore points create snapshots of your system files and registry. If a system change causes problems, you can revert to a previous state without losing personal files.",
                    Reason = "No recent restore point found",
                    EstimatedImpact = "Low",
                    Difficulty = "Easy",
                    RiskLevel = RiskLevel.None,
                    EstimatedCompletionTimeSeconds = 60,
                    SuggestedAction = "restore_create_point",
                    Category = "Safety",
                    Priority = RecommendationPriority.High,
                    RequiredLicenseTier = LicenseTier.Free,
                    SystemArea = "Security",
                    HasImplementation = _implementedTweaks.Contains("restore_create_point")
                });
            }

            // Check if HA GPU Scheduling is disabled
            if (!scanResult.HAGpuSchedulingEnabled)
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_ha_gpu",
                    Title = "Enable Hardware Accelerated GPU Scheduling",
                    Description = "This feature can improve GPU performance by reducing latency and improving frame rates.",
                    DetailedExplanation = "Hardware Accelerated GPU Scheduling reduces latency and improves frame rates by allowing the GPU to manage its own memory. This can provide smoother gameplay and better GPU performance in supported applications.",
                    Reason = "Hardware Accelerated GPU Scheduling is disabled",
                    EstimatedImpact = "Medium",
                    Difficulty = "Medium",
                    RiskLevel = RiskLevel.Medium,
                    EstimatedCompletionTimeSeconds = 30,
                    SuggestedAction = "enable_ha_gpu_scheduling",
                    Category = "Gaming",
                    Priority = RecommendationPriority.Low,
                    RequiredLicenseTier = LicenseTier.Pro,
                    SystemArea = "Gaming",
                    HasImplementation = false
                });
            }

            // Check high RAM usage
            if (scanResult.RamUsagePercent > 85)
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_high_ram",
                    Title = "High Memory Usage Detected",
                    Description = "Your system is using over 85% of available RAM. Consider closing unnecessary applications.",
                    DetailedExplanation = "High memory usage can lead to system slowdowns as Windows uses the page file more frequently. Closing unnecessary applications or adding more RAM can improve system responsiveness.",
                    Reason = $"RAM usage is at {scanResult.RamUsagePercent:F0}%",
                    EstimatedImpact = "Medium",
                    Difficulty = "Easy",
                    RiskLevel = RiskLevel.Low,
                    EstimatedCompletionTimeSeconds = 5,
                    SuggestedAction = "check_running_apps",
                    Category = "System",
                    Priority = RecommendationPriority.Medium,
                    RequiredLicenseTier = LicenseTier.Free,
                    SystemArea = "Performance",
                    HasImplementation = false
                });
            }

            // Check for long Windows uptime
            if (scanResult.WindowsUptimeHours > 168) // 7 days
            {
                recommendations.Add(new Recommendation
                {
                    Id = "rec_long_uptime",
                    Title = "Consider Restarting System",
                    Description = "Your system has been running for over 7 days. A restart can help maintain optimal performance.",
                    DetailedExplanation = "Extended uptime can lead to memory leaks, accumulated temporary files, and driver issues. Regular restarts help maintain system stability and performance.",
                    Reason = $"System uptime is {scanResult.WindowsUptimeHours:F0} hours",
                    EstimatedImpact = "Low",
                    Difficulty = "Easy",
                    RiskLevel = RiskLevel.None,
                    EstimatedCompletionTimeSeconds = 300,
                    SuggestedAction = "restart_system",
                    Category = "Maintenance",
                    Priority = RecommendationPriority.Low,
                    RequiredLicenseTier = LicenseTier.Free,
                    SystemArea = "Maintenance",
                    HasImplementation = false
                });
            }

            _logger.LogInformation("Generated {Count} recommendations", recommendations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recommendations");
        }

        return recommendations;
    }

    /// <summary>
    /// Filters recommendations by priority.
    /// </summary>
    public List<Recommendation> FilterByPriority(List<Recommendation> recommendations, RecommendationPriority minPriority)
    {
        return recommendations.Where(r => r.Priority >= minPriority).ToList();
    }

    /// <summary>
    /// Filters recommendations by category.
    /// </summary>
    public List<Recommendation> FilterByCategory(List<Recommendation> recommendations, string category)
    {
        return recommendations.Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    /// <summary>
    /// Filters recommendations by license tier.
    /// </summary>
    public List<Recommendation> FilterByLicenseTier(List<Recommendation> recommendations, LicenseTier maxTier)
    {
        return recommendations.Where(r => r.RequiredLicenseTier <= maxTier).ToList();
    }

    /// <summary>
    /// Filters recommendations by system area.
    /// </summary>
    public List<Recommendation> FilterBySystemArea(List<Recommendation> recommendations, string systemArea)
    {
        return recommendations.Where(r => r.SystemArea.Equals(systemArea, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    /// <summary>
    /// Groups recommendations by category.
    /// </summary>
    public Dictionary<string, List<Recommendation>> GroupByCategory(List<Recommendation> recommendations)
    {
        return recommendations.GroupBy(r => r.Category).ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Groups recommendations by priority.
    /// </summary>
    public Dictionary<RecommendationPriority, List<Recommendation>> GroupByPriority(List<Recommendation> recommendations)
    {
        return recommendations.GroupBy(r => r.Priority).ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Groups recommendations by system area.
    /// </summary>
    public Dictionary<string, List<Recommendation>> GroupBySystemArea(List<Recommendation> recommendations)
    {
        return recommendations.GroupBy(r => r.SystemArea).ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Sorts recommendations by priority.
    /// </summary>
    public List<Recommendation> SortByPriority(List<Recommendation> recommendations)
    {
        return recommendations.OrderByDescending(r => r.Priority).ToList();
    }
}
