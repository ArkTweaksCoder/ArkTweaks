namespace ArkTweaks.Models;

/// <summary>
/// Represents the overall system health score with category breakdowns.
/// </summary>
public class HealthScore
{
    /// <summary>
    /// Overall health score from 0 to 100
    /// </summary>
    public int OverallScore { get; set; }

    /// <summary>
    /// Storage health score (0-100)
    /// </summary>
    public int StorageScore { get; set; }

    /// <summary>
    /// Startup health score (0-100)
    /// </summary>
    public int StartupScore { get; set; }

    /// <summary>
    /// Cleanup health score (0-100)
    /// </summary>
    public int CleanupScore { get; set; }

    /// <summary>
    /// Power plan health score (0-100)
    /// </summary>
    public int PowerScore { get; set; }

    /// <summary>
    /// Restore status health score (0-100)
    /// </summary>
    public int RestoreScore { get; set; }

    /// <summary>
    /// Memory health score (0-100)
    /// </summary>
    public int MemoryScore { get; set; }

    /// <summary>
    /// Summary description of the health status
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Health status label (Excellent, Good, Fair, Poor)
    /// </summary>
    public string StatusLabel { get; set; } = string.Empty;
}
