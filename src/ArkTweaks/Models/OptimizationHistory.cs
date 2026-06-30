using System;

namespace ArkTweaks.Models;

/// <summary>
/// Represents a history entry for optimization operations.
/// </summary>
public class OptimizationHistory
{
    /// <summary>
    /// Timestamp when optimization was performed
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Tweak ID that was optimized
    /// </summary>
    public string TweakId { get; set; } = string.Empty;

    /// <summary>
    /// Tweak name
    /// </summary>
    public string TweakName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the optimization was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Error message if optimization failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Space reclaimed in bytes (if applicable)
    /// </summary>
    public long SpaceReclaimedBytes { get; set; }
}
