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

    public RecommendationEngine(ILogger<RecommendationEngine> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates recommendations based on system scan results.
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
                    Reason = $"Found {scanResult.TempFilesSizeMB:F0}MB of temporary files",
                    EstimatedImpact = "Medium",
                    RiskLevel = RiskLevel.Low,
                    SuggestedAction = "cleanup_temp_files",
                    Category = "Cleanup"
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
                    Reason = $"{scanResult.StartupAppCount} applications are set to launch at startup",
                    EstimatedImpact = "High",
                    RiskLevel = RiskLevel.Low,
                    SuggestedAction = "manage_startup_apps",
                    Category = "Startup"
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
                    Reason = "Currently using Balanced power plan",
                    EstimatedImpact = "Medium",
                    RiskLevel = RiskLevel.Low,
                    SuggestedAction = "power_high_performance",
                    Category = "Power"
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
                    Reason = $"Disk usage is at {scanResult.DiskUsagePercent:F0}%",
                    EstimatedImpact = "High",
                    RiskLevel = RiskLevel.Low,
                    SuggestedAction = "cleanup_storage",
                    Category = "Storage"
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
                    Reason = "Game Mode is currently disabled",
                    EstimatedImpact = "Medium",
                    RiskLevel = RiskLevel.Low,
                    SuggestedAction = "enable_game_mode",
                    Category = "Gaming"
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
                    Reason = "Storage Sense is currently disabled",
                    EstimatedImpact = "Medium",
                    RiskLevel = RiskLevel.Low,
                    SuggestedAction = "enable_storage_sense",
                    Category = "Storage"
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
                    Reason = "No recent restore point found",
                    EstimatedImpact = "Low",
                    RiskLevel = RiskLevel.None,
                    SuggestedAction = "restore_create_point",
                    Category = "Safety"
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
                    Reason = "Hardware Accelerated GPU Scheduling is disabled",
                    EstimatedImpact = "Medium",
                    RiskLevel = RiskLevel.Medium,
                    SuggestedAction = "enable_ha_gpu_scheduling",
                    Category = "Gaming"
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
                    Reason = $"RAM usage is at {scanResult.RamUsagePercent:F0}%",
                    EstimatedImpact = "Medium",
                    RiskLevel = RiskLevel.Low,
                    SuggestedAction = "check_running_apps",
                    Category = "System"
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
}
