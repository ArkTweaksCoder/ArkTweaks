using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ArkTweaks.Models;

namespace ArkTweaks.Services;

/// <summary>
/// Service for tracking and managing performance history.
/// </summary>
public class PerformanceHistoryService
{
    private readonly ILogger<PerformanceHistoryService> _logger;
    private readonly string _historyFilePath;

    public PerformanceHistoryService(ILogger<PerformanceHistoryService> logger)
    {
        _logger = logger;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var historyDir = Path.Combine(appDataPath, "ArkTweaks", "History");
        Directory.CreateDirectory(historyDir);
        
        _historyFilePath = Path.Combine(historyDir, "performance_history.json");
    }

    /// <summary>
    /// Records a performance snapshot.
    /// </summary>
    public void RecordSnapshot(PerformanceSnapshot snapshot)
    {
        try
        {
            var history = LoadHistory();
            
            history.Add(snapshot);
            
            // Keep only last 1000 snapshots
            if (history.Count > 1000)
            {
                history = history.TakeLast(1000).ToList();
            }
            
            SaveHistory(history);
            _logger.LogDebug("Recorded performance snapshot at {Timestamp}", snapshot.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record performance snapshot");
        }
    }

    /// <summary>
    /// Gets all performance snapshots.
    /// </summary>
    public List<PerformanceSnapshot> GetSnapshots()
    {
        return LoadHistory();
    }

    /// <summary>
    /// Gets snapshots for a specific date range.
    /// </summary>
    public List<PerformanceSnapshot> GetSnapshots(DateTime startDate, DateTime endDate)
    {
        return LoadHistory()
            .Where(s => s.Timestamp >= startDate && s.Timestamp <= endDate)
            .ToList();
    }

    /// <summary>
    /// Gets the most recent N snapshots.
    /// </summary>
    public List<PerformanceSnapshot> GetRecentSnapshots(int count)
    {
        return LoadHistory().TakeLast(count).ToList();
    }

    /// <summary>
    /// Calculates trend for health score.
    /// </summary>
    public PerformanceTrend CalculateHealthScoreTrend(int days = 7)
    {
        var startDate = DateTime.Now.AddDays(-days);
        var snapshots = GetSnapshots(startDate, DateTime.Now);
        
        if (snapshots.Count < 2)
        {
            return new PerformanceTrend
            {
                TrendType = TrendType.InsufficientData,
                ChangeValue = 0,
                PercentageChange = 0
            };
        }

        var oldest = snapshots.First().HealthScore;
        var newest = snapshots.Last().HealthScore;
        var change = newest - oldest;
        var percentage = oldest > 0 ? (change * 100.0 / oldest) : 0;

        return new PerformanceTrend
        {
            TrendType = change > 0 ? TrendType.Improving : change < 0 ? TrendType.Degrading : TrendType.Stable,
            ChangeValue = change,
            PercentageChange = percentage
        };
    }

    /// <summary>
    /// Calculates average RAM usage over a period.
    /// </summary>
    public double CalculateAverageRamUsage(int days = 7)
    {
        var startDate = DateTime.Now.AddDays(-days);
        var snapshots = GetSnapshots(startDate, DateTime.Now);
        
        if (snapshots.Count == 0)
            return 0;

        return snapshots.Average(s => s.RamUsagePercent);
    }

    /// <summary>
    /// Calculates average CPU usage over a period.
    /// </summary>
    public double CalculateAverageCpuUsage(int days = 7)
    {
        var startDate = DateTime.Now.AddDays(-days);
        var snapshots = GetSnapshots(startDate, DateTime.Now);
        
        if (snapshots.Count == 0)
            return 0;

        return snapshots.Average(s => s.CpuUsagePercent);
    }

    /// <summary>
    /// Calculates storage trend.
    /// </summary>
    public PerformanceTrend CalculateStorageTrend(int days = 7)
    {
        var startDate = DateTime.Now.AddDays(-days);
        var snapshots = GetSnapshots(startDate, DateTime.Now);
        
        if (snapshots.Count < 2)
        {
            return new PerformanceTrend
            {
                TrendType = TrendType.InsufficientData,
                ChangeValue = 0,
                PercentageChange = 0
            };
        }

        var oldest = snapshots.First().StorageAvailableGB;
        var newest = snapshots.Last().StorageAvailableGB;
        var change = newest - oldest;
        var percentage = oldest > 0 ? (change * 100.0 / oldest) : 0;

        return new PerformanceTrend
        {
            TrendType = change > 0 ? TrendType.Improving : change < 0 ? TrendType.Degrading : TrendType.Stable,
            ChangeValue = change,
            PercentageChange = percentage
        };
    }

    /// <summary>
    /// Calculates startup app count trend.
    /// </summary>
    public PerformanceTrend CalculateStartupAppTrend(int days = 7)
    {
        var startDate = DateTime.Now.AddDays(-days);
        var snapshots = GetSnapshots(startDate, DateTime.Now);
        
        if (snapshots.Count < 2)
        {
            return new PerformanceTrend
            {
                TrendType = TrendType.InsufficientData,
                ChangeValue = 0,
                PercentageChange = 0
            };
        }

        var oldest = snapshots.First().StartupAppCount;
        var newest = snapshots.Last().StartupAppCount;
        var change = newest - oldest;
        var percentage = oldest > 0 ? (change * 100.0 / oldest) : 0;

        return new PerformanceTrend
        {
            TrendType = change < 0 ? TrendType.Improving : change > 0 ? TrendType.Degrading : TrendType.Stable,
            ChangeValue = change,
            PercentageChange = percentage
        };
    }

    /// <summary>
    /// Gets optimization count over a period.
    /// </summary>
    public int GetOptimizationCount(int days = 7)
    {
        // This would need to be tracked separately
        // For now, return 0 as placeholder
        return 0;
    }

    /// <summary>
    /// Clears all performance history.
    /// </summary>
    public void ClearHistory()
    {
        try
        {
            if (File.Exists(_historyFilePath))
            {
                File.Delete(_historyFilePath);
            }
            _logger.LogInformation("Performance history cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear performance history");
        }
    }

    private List<PerformanceSnapshot> LoadHistory()
    {
        try
        {
            if (File.Exists(_historyFilePath))
            {
                var json = File.ReadAllText(_historyFilePath);
                return JsonSerializer.Deserialize<List<PerformanceSnapshot>>(json) ?? new List<PerformanceSnapshot>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load performance history");
        }
        return new List<PerformanceSnapshot>();
    }

    private void SaveHistory(List<PerformanceSnapshot> history)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(history, options);
            File.WriteAllText(_historyFilePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save performance history");
        }
    }
}

/// <summary>
/// Represents a performance trend analysis.
/// </summary>
public class PerformanceTrend
{
    /// <summary>
    /// Type of trend
    /// </summary>
    public TrendType TrendType { get; set; }

    /// <summary>
    /// Absolute change value
    /// </summary>
    public double ChangeValue { get; set; }

    /// <summary>
    /// Percentage change
    /// </summary>
    public double PercentageChange { get; set; }
}

/// <summary>
/// Types of performance trends.
/// </summary>
public enum TrendType
{
    Improving,
    Degrading,
    Stable,
    InsufficientData
}
