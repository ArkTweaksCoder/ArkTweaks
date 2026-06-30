using System;
using System.Collections.Generic;

namespace ArkTweaks.Models;

/// <summary>
/// Result of a cleanup operation.
/// </summary>
public class CleanupResult
{
    /// <summary>
    /// Whether the cleanup was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Total space reclaimed in bytes
    /// </summary>
    public long SpaceReclaimedBytes { get; set; }

    /// <summary>
    /// Total number of files deleted
    /// </summary>
    public int FilesDeleted { get; set; }

    /// <summary>
    /// Execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Error message if cleanup failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Individual category results
    /// </summary>
    public List<CategoryCleanupResult> CategoryResults { get; set; } = new();

    /// <summary>
    /// Timestamp when cleanup was performed
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Space reclaimed in human-readable format
    /// </summary>
    public string SpaceReclaimedDisplay => FormatBytes(SpaceReclaimedBytes);

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

/// <summary>
/// Result for a single category cleanup.
/// </summary>
public class CategoryCleanupResult
{
    /// <summary>
    /// Category ID
    /// </summary>
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this category was cleaned successfully
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Space reclaimed in bytes
    /// </summary>
    public long SpaceReclaimedBytes { get; set; }

    /// <summary>
    /// Number of files deleted
    /// </summary>
    public int FilesDeleted { get; set; }

    /// <summary>
    /// Error message if cleanup failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
