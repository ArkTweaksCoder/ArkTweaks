using System;
using ArkTweaks.Core.Safety;

namespace ArkTweaks.Models;

/// <summary>
/// Represents a cleanup category with scan and clean capabilities.
/// </summary>
public class CleanupCategory
{
    /// <summary>
    /// Unique identifier for the category
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this category cleans
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Estimated size in bytes
    /// </summary>
    public long EstimatedSizeBytes { get; set; }

    /// <summary>
    /// Number of files found
    /// </summary>
    public int FileCount { get; set; }

    /// <summary>
    /// Risk level of cleaning this category
    /// </summary>
    public RiskLevel RiskLevel { get; set; }

    /// <summary>
    /// Duration of the last scan in milliseconds
    /// </summary>
    public long ScanDurationMs { get; set; }

    /// <summary>
    /// Last time this category was cleaned
    /// </summary>
    public DateTime? LastCleanedTime { get; set; }

    /// <summary>
    /// Whether this category is selected for cleaning
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Whether this category has been scanned
    /// </summary>
    public bool IsScanned { get; set; }

    /// <summary>
    /// Category group (System, Browsers, System Logs, Storage)
    /// </summary>
    public string CategoryGroup { get; set; } = string.Empty;

    /// <summary>
    /// Estimated size in human-readable format
    /// </summary>
    public string EstimatedSizeDisplay => FormatBytes(EstimatedSizeBytes);

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
