using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArkTweaks.Models;
using ArkTweaks.Core.Engine;

namespace ArkTweaks.Services;

/// <summary>
/// Service for executing recommendations with proper integration with OptimizationEngine.
/// </summary>
public class RecommendationExecutionService
{
    private readonly ILogger<RecommendationExecutionService> _logger;
    private readonly OptimizationEngine _optimizationEngine;
    private readonly RecommendationHistoryService _historyService;

    public RecommendationExecutionService(
        ILogger<RecommendationExecutionService> logger,
        OptimizationEngine optimizationEngine,
        RecommendationHistoryService historyService)
    {
        _logger = logger;
        _optimizationEngine = optimizationEngine;
        _historyService = historyService;
    }

    /// <summary>
    /// Executes a recommendation if it has an implemented tweak.
    /// </summary>
    public async Task<RecommendationExecutionResult> ExecuteRecommendationAsync(Recommendation recommendation)
    {
        var result = new RecommendationExecutionResult
        {
            RecommendationId = recommendation.Id,
            RecommendationTitle = recommendation.Title,
            Timestamp = DateTime.UtcNow
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            if (!recommendation.HasImplementation)
            {
                result.Success = false;
                result.ErrorMessage = "Coming Soon - This recommendation does not have an implemented tweak yet.";
                _logger.LogInformation("Recommendation {Title} has no implementation", recommendation.Title);
                return result;
            }

            _logger.LogInformation("Executing recommendation: {Title}", recommendation.Title);

            var tweakResult = await _optimizationEngine.ExecuteTweakAsync(recommendation.SuggestedAction);

            stopwatch.Stop();
            result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            result.Success = tweakResult.Success;
            result.ErrorMessage = tweakResult.ErrorMessage;

            // Record in history
            _historyService.RecordExecution(new RecommendationHistory
            {
                Timestamp = result.Timestamp,
                RecommendationId = recommendation.Id,
                Title = recommendation.Title,
                Action = RecommendationAction.Executed,
                Success = result.Success,
                ExecutionTimeMs = result.ExecutionTimeMs,
                ErrorMessage = result.ErrorMessage
            });

            _logger.LogInformation("Recommendation {Title} execution completed: {Success}", recommendation.Title, result.Success);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error executing recommendation {Title}", recommendation.Title);

            // Record failure in history
            _historyService.RecordExecution(new RecommendationHistory
            {
                Timestamp = result.Timestamp,
                RecommendationId = recommendation.Id,
                Title = recommendation.Title,
                Action = RecommendationAction.Executed,
                Success = false,
                ExecutionTimeMs = result.ExecutionTimeMs,
                ErrorMessage = result.ErrorMessage
            });
        }

        return result;
    }

    /// <summary>
    /// Skips a recommendation and records it in history.
    /// </summary>
    public void SkipRecommendation(Recommendation recommendation)
    {
        _historyService.RecordExecution(new RecommendationHistory
        {
            Timestamp = DateTime.UtcNow,
            RecommendationId = recommendation.Id,
            Title = recommendation.Title,
            Action = RecommendationAction.Skipped,
            Success = true,
            ExecutionTimeMs = 0
        });

        _logger.LogInformation("Recommendation {Title} skipped by user", recommendation.Title);
    }

    /// <summary>
    /// Dismisses a recommendation and records it in history.
    /// </summary>
    public void DismissRecommendation(Recommendation recommendation)
    {
        _historyService.RecordExecution(new RecommendationHistory
        {
            Timestamp = DateTime.UtcNow,
            RecommendationId = recommendation.Id,
            Title = recommendation.Title,
            Action = RecommendationAction.Dismissed,
            Success = true,
            ExecutionTimeMs = 0
        });

        _logger.LogInformation("Recommendation {Title} dismissed by user", recommendation.Title);
    }
}

/// <summary>
/// Result of a recommendation execution.
/// </summary>
public class RecommendationExecutionResult
{
    /// <summary>
    /// Recommendation ID
    /// </summary>
    public string RecommendationId { get; set; } = string.Empty;

    /// <summary>
    /// Recommendation title
    /// </summary>
    public string RecommendationTitle { get; set; } = string.Empty;

    /// <summary>
    /// Whether execution was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if execution failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Timestamp when execution was performed
    /// </summary>
    public DateTime Timestamp { get; set; }
}
