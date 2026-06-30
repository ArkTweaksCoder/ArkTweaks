using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace ArkTweaks.Services;

public class StartupService
{
    private readonly ILogger<StartupService> _logger;

    public StartupService(ILogger<StartupService> logger)
    {
        _logger = logger;
    }

    public class StartupItem
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }

    public List<StartupItem> GetStartupItems()
    {
        var items = new List<StartupItem>();

        try
        {
            var runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            
            if (runKey != null)
            {
                foreach (var name in runKey.GetValueNames())
                {
                    var value = runKey.GetValue(name)?.ToString();
                    
                    if (!string.IsNullOrEmpty(value))
                    {
                        items.Add(new StartupItem
                        {
                            Name = name,
                            Path = value,
                            Enabled = true
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get startup items");
        }

        return items;
    }

    public void DisableStartupItem(string name)
    {
        try
        {
            var runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            
            if (runKey != null)
            {
                runKey.DeleteValue(name, false);
                _logger.LogInformation("Disabled startup item: {Name}", name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disable startup item: {Name}", name);
            throw;
        }
    }

    public void EnableStartupItem(string name, string path)
    {
        try
        {
            var runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            
            if (runKey != null)
            {
                runKey.SetValue(name, path);
                _logger.LogInformation("Enabled startup item: {Name}", name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enable startup item: {Name}", name);
            throw;
        }
    }
}
