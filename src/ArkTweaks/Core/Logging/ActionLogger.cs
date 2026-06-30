using System;
using System.IO;
using System.Text.Json;

namespace ArkTweaks.Core.Logging;

public class ActionLogger
{
    private readonly string _logFilePath;
    private readonly object _lock = new();

    public ActionLogger()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var logDir = Path.Combine(appDataPath, "ArkTweaks", "Logs");
        Directory.CreateDirectory(logDir);
        
        _logFilePath = Path.Combine(logDir, $"actions_{DateTime.UtcNow:yyyyMMdd}.jsonl");
    }

    public void LogAction(string tweakId, string tweakName, bool success, string? errorMessage = null)
    {
        var entry = new
        {
            Timestamp = DateTime.UtcNow,
            TweakId = tweakId,
            TweakName = tweakName,
            Success = success,
            ErrorMessage = errorMessage
        };

        lock (_lock)
        {
            var json = JsonSerializer.Serialize(entry);
            File.AppendAllText(_logFilePath, json + Environment.NewLine);
        }
    }

    public List<Dictionary<string, object>> GetRecentActions(int count = 100)
    {
        if (!File.Exists(_logFilePath))
        {
            return new List<Dictionary<string, object>>();
        }

        lock (_lock)
        {
            var lines = File.ReadAllLines(_logFilePath);
            var entries = new List<Dictionary<string, object>>();

            foreach (var line in lines.TakeLast(count))
            {
                try
                {
                    var entry = JsonSerializer.Deserialize<Dictionary<string, object>>(line);
                    if (entry != null)
                    {
                        entries.Add(entry);
                    }
                }
                catch
                {
                    // Skip malformed lines
                }
            }

            return entries;
        }
    }
}
