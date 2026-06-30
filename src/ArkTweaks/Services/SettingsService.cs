using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ArkTweaks.Models;

namespace ArkTweaks.Services;

/// <summary>
/// Service for managing application settings persistence and loading.
/// </summary>
public class SettingsService
{
    private readonly ILogger<SettingsService> _logger;
    private readonly string _settingsFilePath;
    private Settings _currentSettings;

    public SettingsService(ILogger<SettingsService> logger)
    {
        _logger = logger;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var settingsDir = Path.Combine(appDataPath, "ArkTweaks");
        Directory.CreateDirectory(settingsDir);
        
        _settingsFilePath = Path.Combine(settingsDir, "settings.json");
        _currentSettings = LoadSettings();
    }

    /// <summary>
    /// Gets the current settings.
    /// </summary>
    public Settings CurrentSettings => _currentSettings;

    /// <summary>
    /// Loads settings from file.
    /// </summary>
    private Settings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<Settings>(json);
                if (settings != null)
                {
                    _logger.LogInformation("Settings loaded successfully");
                    return settings;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading settings, using defaults");
        }

        // Return default settings if file doesn't exist or error occurred
        return new Settings();
    }

    /// <summary>
    /// Saves current settings to file.
    /// </summary>
    public bool SaveSettings()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            var json = JsonSerializer.Serialize(_currentSettings, options);
            File.WriteAllText(_settingsFilePath, json);
            
            _logger.LogInformation("Settings saved successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving settings");
            return false;
        }
    }

    /// <summary>
    /// Updates a specific setting value.
    /// </summary>
    public void UpdateSetting<T>(string propertyName, T value)
    {
        try
        {
            var property = typeof(Settings).GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(_currentSettings, value);
                SaveSettings();
                _logger.LogDebug("Updated setting {Property} to {Value}", propertyName, value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating setting {Property}", propertyName);
        }
    }

    /// <summary>
    /// Resets all settings to defaults.
    /// </summary>
    public void ResetToDefaults()
    {
        _currentSettings = new Settings();
        SaveSettings();
        _logger.LogInformation("Settings reset to defaults");
    }

    /// <summary>
    /// Gets the settings file path.
    /// </summary>
    public string SettingsFilePath => _settingsFilePath;
}
