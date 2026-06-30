using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ArkTweaks.Models;

namespace ArkTweaks.Services;

/// <summary>
/// Service for tracking and managing cleanup history.
/// </summary>
public class CleanupHistoryService
{
    private readonly ILogger<CleanupHistoryService> _logger;
    private readonly string _historyFilePath;

    public CleanupHistoryService(ILogger<CleanupHistoryService> logger)
    {
        _logger = logger;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var historyDir = Path.Combine(appDataPath, "ArkTweaks", "History");
        Directory.CreateDirectory(historyDir);
        
        _historyFilePath = Path.Combine(historyDir, "cleanup_history.json");
    }

    /// <summary>
    /// Records a cleanup operation in history.
    /// </summary>
    public void RecordCleanup(CleanupResult result)
    {
        try
        {
            var history = LoadHistory();
            
            var entry = new CleanupHistoryEntry
            {
                Timestamp = result.Timestamp,
                CategoriesCleaned = result.CategoryResults.Select(r => r.CategoryName).ToList(),
                SpaceReclaimedBytes = result.SpaceReclaimedBytes,
                FilesDeleted = result.FilesDeleted,
                ExecutionTimeMs = result.ExecutionTimeMs,
                Success = result.Success,
                ErrorMessage = result.ErrorMessage
            };
            
            history.Add(entry);
            
            // Keep only last 100 entries
            if (history.Count > 100)
            {
                history = history.TakeLast(100).ToList();
            }
            
            SaveHistory(history);
            _logger.LogInformation("Recorded cleanup history: {SpaceReclaimed} reclaimed", result.SpaceReclaimedDisplay);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record cleanup history");
        }
    }

    /// <summary>
    /// Gets all cleanup history entries.
    /// </summary>
    public List<CleanupHistoryEntry> GetHistory()
    {
        return LoadHistory();
    }

    /// <summary>
    /// Gets cleanup history for a specific date range.
    /// </summary>
    public List<CleanupHistoryEntry> GetHistory(DateTime startDate, DateTime endDate)
    {
        return LoadHistory()
            .Where(h => h.Timestamp >= startDate && h.Timestamp <= endDate)
            .ToList();
    }

    /// <summary>
    /// Gets total space reclaimed from all cleanups.
    /// </summary>
    public long GetTotalSpaceReclaimed()
    {
        return LoadHistory().Sum(h => h.SpaceReclaimedBytes);
    }

    /// <summary>
    /// Gets total files deleted from all cleanups.
    /// </summary>
    public int GetTotalFilesDeleted()
    {
        return LoadHistory().Sum(h => h.FilesDeleted);
    }

    /// <summary>
    /// Clears all cleanup history.
    /// </summary>
    public void ClearHistory()
    {
        try
        {
            if (File.Exists(_historyFilePath))
            {
                File.Delete(_historyFilePath);
            }
            _logger.LogInformation("Cleanup history cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear cleanup history");
        }
    }

    private List<CleanupHistoryEntry> LoadHistory()
    {
        try
        {
            if (File.Exists(_historyFilePath))
            {
                var json = File.ReadAllText(_historyFilePath);
                return JsonSerializer.Deserialize<List<CleanupHistoryEntry>>(json) ?? new List<CleanupHistoryEntry>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load cleanup history");
        }
        return new List<CleanupHistoryEntry>();
    }

    private void SaveHistory(List<CleanupHistoryEntry> history)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(history, options);
            File.WriteAllText(_historyFilePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save cleanup history");
        }
    }
}

/// <summary>
/// Represents a single cleanup history entry.
/// </summary>
public class CleanupHistoryEntry
{
    public DateTime Timestamp { get; set; }
    public List<string> CategoriesCleaned { get; set; } = new();
    public long SpaceReclaimedBytes { get; set; }
    public int FilesDeleted { get; set; }
    public long ExecutionTimeMs { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
