using ArkTweaks.Core.Safety;

namespace ArkTweaks.Models;

/// <summary>
/// Represents a system optimization recommendation.
/// </summary>
public class Recommendation
{
    /// <summary>
    /// Unique identifier for the recommendation
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Title of the recommendation
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the recommendation
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Detailed explanation of the recommendation
    /// </summary>
    public string DetailedExplanation { get; set; } = string.Empty;

    /// <summary>
    /// Reason why this recommendation is being made
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Estimated impact (Low, Medium, High)
    /// </summary>
    public string EstimatedImpact { get; set; } = string.Empty;

    /// <summary>
    /// Difficulty level (Easy, Medium, Hard)
    /// </summary>
    public string Difficulty { get; set; } = string.Empty;

    /// <summary>
    /// Risk level of the suggested action
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// Estimated completion time in seconds
    /// </summary>
    public int EstimatedCompletionTimeSeconds { get; set; }

    /// <summary>
    /// Suggested action to take (e.g., tweak ID or action description)
    /// </summary>
    public string SuggestedAction { get; set; } = string.Empty;

    /// <summary>
    /// Category of the recommendation
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Priority level (Critical, High, Medium, Low, Informational)
    /// </summary>
    public RecommendationPriority Priority { get; set; }

    /// <summary>
    /// License tier required (Free, Pro, Enterprise)
    /// </summary>
    public LicenseTier RequiredLicenseTier { get; set; }

    /// <summary>
    /// System area affected (Performance, Storage, Gaming, Security, Maintenance, Power, Windows)
    /// </summary>
    public string SystemArea { get; set; } = string.Empty;

    /// <summary>
    /// Whether this recommendation has been applied
    /// </summary>
    public bool IsApplied { get; set; }

    /// <summary>
    /// Whether this recommendation has a corresponding tweak implementation
    /// </summary>
    public bool HasImplementation { get; set; }
}

/// <summary>
/// Priority levels for recommendations.
/// </summary>
public enum RecommendationPriority
{
    Critical,
    High,
    Medium,
    Low,
    Informational
}
