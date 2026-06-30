using System;
using Microsoft.Extensions.Logging;
using ArkTweaks.Models;

namespace ArkTweaks.Services;

/// <summary>
/// Service for calculating system health scores based on scan results.
/// </summary>
public class HealthScoreService
{
    private readonly ILogger<HealthScoreService> _logger;

    public HealthScoreService(ILogger<HealthScoreService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Calculates health score from system scan results.
    /// </summary>
    public HealthScore CalculateHealthScore(SystemScanResult scanResult)
    {
        var score = new HealthScore();

        try
        {
            // Storage Score (0-100)
            // Penalize high disk usage
            score.StorageScore = CalculateStorageScore(scanResult.DiskUsagePercent);

            // Startup Score (0-100)
            // Penalize too many startup apps
            score.StartupScore = CalculateStartupScore(scanResult.StartupAppCount);

            // Cleanup Score (0-100)
            // Penalize large temp files
            score.CleanupScore = CalculateCleanupScore(scanResult.TempFilesSizeMB);

            // Power Score (0-100)
            // Reward high performance, penalize balanced/power saver
            score.PowerScore = CalculatePowerScore(scanResult.CurrentPowerPlan);

            // Restore Score (0-100)
            // Reward having restore points
            score.RestoreScore = CalculateRestoreScore(scanResult.HasRestorePoint);

            // Memory Score (0-100)
            // Penalize high memory usage
            score.MemoryScore = CalculateMemoryScore(scanResult.RamUsagePercent);

            // Overall Score (weighted average)
            score.OverallScore = CalculateOverallScore(
                score.StorageScore,
                score.StartupScore,
                score.CleanupScore,
                score.PowerScore,
                score.RestoreScore,
                score.MemoryScore
            );

            // Status Label
            score.StatusLabel = GetStatusLabel(score.OverallScore);

            // Summary
            score.Summary = GenerateSummary(score, scanResult);

            _logger.LogInformation("Health score calculated: {Score}", score.OverallScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating health score");
        }

        return score;
    }

    private int CalculateStorageScore(double diskUsagePercent)
    {
        if (diskUsagePercent < 50) return 100;
        if (diskUsagePercent < 70) return 80;
        if (diskUsagePercent < 85) return 60;
        if (diskUsagePercent < 95) return 40;
        return 20;
    }

    private int CalculateStartupScore(int startupAppCount)
    {
        if (startupAppCount <= 5) return 100;
        if (startupAppCount <= 10) return 80;
        if (startupAppCount <= 15) return 60;
        if (startupAppCount <= 20) return 40;
        return 20;
    }

    private int CalculateCleanupScore(double tempFilesSizeMB)
    {
        if (tempFilesSizeMB < 100) return 100;
        if (tempFilesSizeMB < 300) return 80;
        if (tempFilesSizeMB < 500) return 60;
        if (tempFilesSizeMB < 1000) return 40;
        return 20;
    }

    private int CalculatePowerScore(string currentPowerPlan)
    {
        return currentPowerPlan switch
        {
            "High Performance" => 100,
            "Ultimate Performance" => 100,
            "Balanced" => 70,
            "Power Saver" => 50,
            _ => 70
        };
    }

    private int CalculateRestoreScore(bool hasRestorePoint)
    {
        return hasRestorePoint ? 100 : 50;
    }

    private int CalculateMemoryScore(double ramUsagePercent)
    {
        if (ramUsagePercent < 50) return 100;
        if (ramUsagePercent < 70) return 90;
        if (ramUsagePercent < 85) return 70;
        if (ramUsagePercent < 95) return 50;
        return 30;
    }

    private int CalculateOverallScore(
        int storageScore,
        int startupScore,
        int cleanupScore,
        int powerScore,
        int restoreScore,
        int memoryScore)
    {
        // Weighted average
        // Storage: 20%, Startup: 15%, Cleanup: 15%, Power: 15%, Restore: 15%, Memory: 20%
        return (int)(
            (storageScore * 0.20) +
            (startupScore * 0.15) +
            (cleanupScore * 0.15) +
            (powerScore * 0.15) +
            (restoreScore * 0.15) +
            (memoryScore * 0.20)
        );
    }

    private string GetStatusLabel(int overallScore)
    {
        return overallScore switch
        {
            >= 90 => "Excellent",
            >= 75 => "Good",
            >= 60 => "Fair",
            >= 40 => "Poor",
            _ => "Critical"
        };
    }

    private string GenerateSummary(HealthScore score, SystemScanResult scanResult)
    {
        var issues = new System.Text.StringBuilder();

        if (score.StorageScore < 70)
            issues.Append($"Storage usage at {scanResult.DiskUsagePercent:F0}%. ");
        if (score.StartupScore < 70)
            issues.Append($"{scanResult.StartupAppCount} startup apps. ");
        if (score.CleanupScore < 70)
            issues.Append($"{scanResult.TempFilesSizeMB:F0}MB temp files. ");
        if (score.PowerScore < 70)
            issues.Append($"Using {scanResult.CurrentPowerPlan} power plan. ");
        if (score.RestoreScore < 70)
            issues.Append("No restore point. ");
        if (score.MemoryScore < 70)
            issues.Append($"RAM usage at {scanResult.RamUsagePercent:F0}%. ");

        if (issues.Length == 0)
            return "Your system is in excellent health. No immediate optimizations needed.";

        return $"System status: {score.StatusLabel}. {issues.ToString()}Consider applying recommendations to improve performance.";
    }
}
