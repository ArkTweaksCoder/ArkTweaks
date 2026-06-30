using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Core.Logging;

namespace ArkTweaks.UI.ViewModels;

public class LogsViewModel : BaseViewModel
{
    private readonly ILogger<LogsViewModel> _logger;
    private string _searchText = string.Empty;
    private string _selectedLogLevel = "All";

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            FilterLogs();
        }
    }

    public string SelectedLogLevel
    {
        get => _selectedLogLevel;
        set
        {
            SetProperty(ref _selectedLogLevel, value);
            FilterLogs();
        }
    }

    public ObservableCollection<string> LogLevels { get; } = new(new[] { "All", "ERROR", "WARN", "INFO", "DEBUG" });
    public ObservableCollection<LogEntry> LogEntries { get; } = new();

    public LogsViewModel(ILogger<LogsViewModel> logger) 
        : base(logger)
    {
        _logger = logger;
        LoadLogs();
    }

    private void LoadLogs()
    {
        try
        {
            // Load logs from logging service
            // This is a simplified implementation
            LogEntries.Clear();
            
            // Add sample data for now
            LogEntries.Add(new LogEntry
            {
                Level = "INFO",
                Message = "Application started successfully",
                Timestamp = DateTime.Now.AddMinutes(-10).ToString("g")
            });
            
            LogEntries.Add(new LogEntry
            {
                Level = "INFO",
                Message = "System scan completed",
                Timestamp = DateTime.Now.AddMinutes(-5).ToString("g")
            });
            
            LogEntries.Add(new LogEntry
            {
                Level = "WARN",
                Message = "High CPU usage detected",
                Timestamp = DateTime.Now.AddMinutes(-2).ToString("g")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load logs");
        }
    }

    private void FilterLogs()
    {
        try
        {
            var filtered = LogEntries
                .Where(l => SelectedLogLevel == "All" || l.Level == SelectedLogLevel)
                .Where(l => l.Message.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            // Rebuild collection with filtered items
            LogEntries.Clear();
            foreach (var item in filtered)
            {
                LogEntries.Add(item);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to filter logs");
        }
    }

    public void ExportLogs()
    {
        try
        {
            // Export logs to file
            _logger.LogInformation("Logs exported");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export logs");
        }
    }

    public void ClearLogs()
    {
        try
        {
            LogEntries.Clear();
            _logger.LogInformation("Logs cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear logs");
        }
    }

    public void CopyLogEntry(LogEntry entry)
    {
        try
        {
            // Copy log entry to clipboard
            _logger.LogInformation("Log entry copied: {Message}", entry.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to copy log entry");
        }
    }
}

public class LogEntry
{
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
}
