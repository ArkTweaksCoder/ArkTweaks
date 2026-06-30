using System;
using System.Collections.Generic;

namespace ArkTweaks.Models;

public class TweakResult
{
    public string TweakId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public long? SpaceFreedBytes { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
