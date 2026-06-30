using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ArkTweaks.Models;

namespace ArkTweaks.Services;

/// <summary>
/// Service for tracking and managing recommendation history.
/// </summary>
public class RecommendationHistoryService
{
    private readonly ILogger<RecommendationHistoryService> _logger;
    private readonly string _historyFilePath;

    public RecommendationHistoryService(ILogger<RecommendationHistoryService> logger)
    {
        _logger = logger;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var historyDir = Path.Combine(appDataPath, "ArkTweaks", "History");
        Directory.CreateDirectory(historyDir);
        
        _historyFilePath = Path.Combine(historyDir, "recommendation_history.json");
    }

    /// <summary>
    /// Records a recommendation action in history.
    /// </summary>
    public void RecordExecution(RecommendationHistory history)
    {
        try
        {
            var historyList = LoadHistory();
            
            historyList.Add(history);
            
            // Keep only last 200 entries
            if (historyList.Count > 200)
            {
                historyList = historyList.TakeLast(200).ToList();
            }
            
            SaveHistory(historyList);
            _logger.LogDebug("Recorded recommendation history: {Action} for {Title}", history.Action, history.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record recommendation history");
        }
    }

    /// <summary>
    /// Gets all recommendation history entries.
    /// </summary>
    public List<RecommendationHistory> GetHistory()
    {
        return LoadHistory();
    }

    /// <summary>
    /// Gets recommendation history for a specific date range.
    /// </summary>
    public List<RecommendationHistory> GetHistory(DateTime startDate, DateTime endDate)
    {
        return LoadHistory()
            .Where(h => h.Timestamp >= startDate && h.Timestamp <= endDate)
            .ToList();
    }

    /// <summary>
    /// Gets history for a specific recommendation.
    /// </summary>
    public List<RecommendationHistory> GetHistoryForRecommendation(string recommendationId)
    {
        return LoadHistory()
            .Where(h => h.RecommendationId == recommendationId)
            .ToList();
    }

    /// <summary>
    /// Gets count of executed recommendations.
    /// </summary>
    public int GetExecutedCount()
    {
        return LoadHistory().Count(h => h.Action == RecommendationAction.Executed);
    }

    /// <summary>
    /// Gets count of skipped recommendations.
    /// </summary>
    public int GetSkippedCount()
    {
        return LoadHistory().Count(h => h.Action == RecommendationAction.Skipped);
    }

    /// <summary>
    /// Gets success rate of executed recommendations.
    /// </summary>
    public double GetSuccessRate()
    {
        var executed = LoadHistory().Where(h => h.Action == RecommendationAction.Executed).ToList();
        if (executed.Count == 0)
            return 0;

        return (double)executed.Count(h => h.Success) / executed.Count * 100;
    }

    /// <summary>
    /// Clears all recommendation history.
    /// </summary>
    public void ClearHistory()
    {
        try
        {
            if (File.Exists(_historyFilePath))
            {
                File.Delete(_historyFilePath);
            }
            _logger.LogInformation("Recommendation history cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear recommendation history");
        }
    }

    private List<RecommendationHistory> LoadHistory()
    {
        try
        {
            if (File.Exists(_historyFilePath))
            {
                var json = File.ReadAllText(_historyFilePath);
                return JsonSerializer.Deserialize<List<RecommendationHistory>>(json) ?? new List<RecommendationHistory>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load recommendation history");
        }
        return new List<RecommendationHistory>();
    }

    private void SaveHistory(List<RecommendationHistory> history)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(history, options);
            File.WriteAllText(_historyFilePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save recommendation history");
        }
    }
}
