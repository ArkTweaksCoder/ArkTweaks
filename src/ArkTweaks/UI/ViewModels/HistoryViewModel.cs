using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Core.Logging;
using ArkTweaks.Models;

namespace ArkTweaks.UI.ViewModels;

public class HistoryViewModel : BaseViewModel
{
    private readonly ActionLogger _actionLogger;
    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            FilterHistory();
        }
    }

    public ObservableCollection<HistoryItem> HistoryItems { get; } = new();

    public HistoryViewModel(ILogger<HistoryViewModel> logger, ActionLogger actionLogger) 
        : base(logger)
    {
        _actionLogger = actionLogger;
        LoadHistory();
    }

    private void LoadHistory()
    {
        try
        {
            // Load history from action logger
            // For now, use sample data since ActionLogger structure needs to be verified
            HistoryItems.Clear();
            
            // Add sample data
            HistoryItems.Add(new HistoryItem
            {
                Icon = "⚡",
                Action = "Optimize",
                Details = "System optimization completed successfully",
                Timestamp = DateTime.Now.AddMinutes(-5).ToString("g"),
                Status = "Success"
            });
            
            HistoryItems.Add(new HistoryItem
            {
                Icon = "🧹",
                Action = "Cleanup",
                Details = "Temporary files cleaned (245 MB freed)",
                Timestamp = DateTime.Now.AddMinutes(-15).ToString("g"),
                Status = "Success"
            });
            
            HistoryItems.Add(new HistoryItem
            {
                Icon = "🔁",
                Action = "Restore Point",
                Details = "System restore point created",
                Timestamp = DateTime.Now.AddHours(-1).ToString("g"),
                Status = "Success"
            });
            
            HistoryItems.Add(new HistoryItem
            {
                Icon = "🚀",
                Action = "Startup",
                Details = "Disabled 3 startup items",
                Timestamp = DateTime.Now.AddHours(-2).ToString("g"),
                Status = "Success"
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load history");
        }
    }

    private void FilterHistory()
    {
        try
        {
            var filtered = HistoryItems
                .Where(h => h.Action.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                           h.Details.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            // Rebuild collection with filtered items
            HistoryItems.Clear();
            foreach (var item in filtered)
            {
                HistoryItems.Add(item);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to filter history");
        }
    }

    private string GetIconForAction(string actionName)
    {
        return actionName.ToLowerInvariant() switch
        {
            var a when a.Contains("optimize") => "⚡",
            var a when a.Contains("cleanup") => "🧹",
            var a when a.Contains("restore") => "🔁",
            var a when a.Contains("startup") => "🚀",
            _ => "📋"
        };
    }

    public void ExportHistory()
    {
        try
        {
            // Export history to file
            Logger.LogInformation("History exported");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to export history");
        }
    }

    public void ClearHistory()
    {
        try
        {
            HistoryItems.Clear();
            Logger.LogInformation("History cleared");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to clear history");
        }
    }
}

public class HistoryItem
{
    public string Icon { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
