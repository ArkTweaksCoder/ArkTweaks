using System;

namespace ArkTweaks.Models;

/// <summary>
/// Represents a history entry for recommendation operations.
/// </summary>
public class RecommendationHistory
{
    /// <summary>
    /// Timestamp when the recommendation was executed
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Recommendation ID
    /// </summary>
    public string RecommendationId { get; set; } = string.Empty;

    /// <summary>
    /// Recommendation title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Action taken (Executed, Skipped, Dismissed)
    /// </summary>
    public RecommendationAction Action { get; set; }

    /// <summary>
    /// Whether the action was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Error message if action failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Actions that can be taken on recommendations.
/// </summary>
public enum RecommendationAction
{
    Executed,
    Skipped,
    Dismissed
}
