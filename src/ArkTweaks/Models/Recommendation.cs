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
    /// Reason why this recommendation is being made
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Estimated impact (Low, Medium, High)
    /// </summary>
    public string EstimatedImpact { get; set; } = string.Empty;

    /// <summary>
    /// Risk level of the suggested action
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// Suggested action to take (e.g., tweak ID or action description)
    /// </summary>
    public string SuggestedAction { get; set; } = string.Empty;

    /// <summary>
    /// Category of the recommendation
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Whether this recommendation has been applied
    /// </summary>
    public bool IsApplied { get; set; }
}
