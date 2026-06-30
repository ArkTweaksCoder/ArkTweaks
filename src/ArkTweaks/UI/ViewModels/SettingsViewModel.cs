using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Models;

namespace ArkTweaks.UI.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly SettingsService _settingsService;
    private readonly LicenseService _licenseService;
    private string _licenseKeyInput = string.Empty;
    private Settings _settings;

    // General Settings
    private bool _launchOnStartup;
    public bool LaunchOnStartup
    {
        get => _launchOnStartup;
        set
        {
            if (_launchOnStartup != value)
            {
                _launchOnStartup = value;
                _settings.LaunchOnStartup = value;
                SetProperty(ref _launchOnStartup, value);
                _settingsService.SaveSettings();
            }
        }
    }

    private bool _autoRefreshDashboard;
    public bool AutoRefreshDashboard
    {
        get => _autoRefreshDashboard;
        set
        {
            if (_autoRefreshDashboard != value)
            {
                _autoRefreshDashboard = value;
                _settings.AutoRefreshDashboard = value;
                SetProperty(ref _autoRefreshDashboard, value);
                _settingsService.SaveSettings();
            }
        }
    }

    private bool _checkForUpdates;
    public bool CheckForUpdates
    {
        get => _checkForUpdates;
        set
        {
            if (_checkForUpdates != value)
            {
                _checkForUpdates = value;
                _settings.CheckForUpdates = value;
                SetProperty(ref _checkForUpdates, value);
                _settingsService.SaveSettings();
            }
        }
    }

    // Appearance Settings
    private AppTheme _theme;
    public AppTheme Theme
    {
        get => _theme;
        set
        {
            if (_theme != value)
            {
                _theme = value;
                _settings.Theme = value;
                SetProperty(ref _theme, value);
                _settingsService.SaveSettings();
            }
        }
    }

    private string _accentColor;
    public string AccentColor
    {
        get => _accentColor;
        set
        {
            if (_accentColor != value)
            {
                _accentColor = value;
                _settings.AccentColor = value;
                SetProperty(ref _accentColor, value);
                _settingsService.SaveSettings();
            }
        }
    }

    // Optimization Settings
    private bool _autoCreateRestorePoint;
    public bool AutoCreateRestorePoint
    {
        get => _autoCreateRestorePoint;
        set
        {
            if (_autoCreateRestorePoint != value)
            {
                _autoCreateRestorePoint = value;
                _settings.AutoCreateRestorePoint = value;
                SetProperty(ref _autoCreateRestorePoint, value);
                _settingsService.SaveSettings();
            }
        }
    }

    private bool _autoBackupRegistry;
    public bool AutoBackupRegistry
    {
        get => _autoBackupRegistry;
        set
        {
            if (_autoBackupRegistry != value)
            {
                _autoBackupRegistry = value;
                _settings.AutoBackupRegistry = value;
                SetProperty(ref _autoBackupRegistry, value);
                _settingsService.SaveSettings();
            }
        }
    }

    private bool _showConfirmationDialogs;
    public bool ShowConfirmationDialogs
    {
        get => _showConfirmationDialogs;
        set
        {
            if (_showConfirmationDialogs != value)
            {
                _showConfirmationDialogs = value;
                _settings.ShowConfirmationDialogs = value;
                SetProperty(ref _showConfirmationDialogs, value);
                _settingsService.SaveSettings();
            }
        }
    }

    // Logs Settings
    private bool _enableLogging;
    public bool EnableLogging
    {
        get => _enableLogging;
        set
        {
            if (_enableLogging != value)
            {
                _enableLogging = value;
                _settings.EnableLogging = value;
                SetProperty(ref _enableLogging, value);
                _settingsService.SaveSettings();
            }
        }
    }

    private int _logRetentionDays;
    public int LogRetentionDays
    {
        get => _logRetentionDays;
        set
        {
            if (_logRetentionDays != value)
            {
                _logRetentionDays = value;
                _settings.LogRetentionDays = value;
                SetProperty(ref _logRetentionDays, value);
                _settingsService.SaveSettings();
            }
        }
    }

    // License Settings
    public string LicenseKeyInput
    {
        get => _licenseKeyInput;
        set => SetProperty(ref _licenseKeyInput, value);
    }

    public LicenseTier CurrentTier => _licenseService.CurrentLicense.Tier;
    public string CurrentLicenseKey => _licenseService.CurrentLicense.LicenseKey;

    // About Settings
    public string Version => _settings.Version;
    public string ApplicationName => _settings.ApplicationName;

    public SettingsViewModel(
        ILogger<SettingsViewModel> logger,
        SettingsService settingsService,
        LicenseService licenseService)
        : base(logger)
    {
        _settingsService = settingsService;
        _licenseService = licenseService;
        _settings = _settingsService.CurrentSettings;
        
        // Initialize backing fields from settings
        _launchOnStartup = _settings.LaunchOnStartup;
        _autoRefreshDashboard = _settings.AutoRefreshDashboard;
        _checkForUpdates = _settings.CheckForUpdates;
        _theme = _settings.Theme;
        _accentColor = _settings.AccentColor;
        _autoCreateRestorePoint = _settings.AutoCreateRestorePoint;
        _autoBackupRegistry = _settings.AutoBackupRegistry;
        _showConfirmationDialogs = _settings.ShowConfirmationDialogs;
        _enableLogging = _settings.EnableLogging;
        _logRetentionDays = _settings.LogRetentionDays;
    }

    public bool ActivateLicense()
    {
        if (string.IsNullOrWhiteSpace(LicenseKeyInput))
            return false;

        var success = _licenseService.ActivateLicense(LicenseKeyInput);
        if (success)
        {
            _settings.LicenseKey = LicenseKeyInput;
            _settings.CurrentTier = _licenseService.CurrentLicense.Tier;
            _settingsService.SaveSettings();
        }
        return success;
    }

    public void DeactivateLicense()
    {
        _licenseService.DeactivateLicense();
        _settings.LicenseKey = string.Empty;
        _settings.CurrentTier = LicenseTier.Free;
        _settingsService.SaveSettings();
    }

    public void OpenLogFolder()
    {
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var logDir = Path.Combine(appDataPath, "ArkTweaks", "Logs");
            
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            
            Process.Start(new ProcessStartInfo
            {
                FileName = logDir,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to open log folder");
        }
    }

    public void ClearLogs()
    {
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var logDir = Path.Combine(appDataPath, "ArkTweaks", "Logs");
            
            if (Directory.Exists(logDir))
            {
                var files = Directory.GetFiles(logDir);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
                Logger.LogInformation("Logs cleared");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to clear logs");
        }
    }

    public void ExportLogs()
    {
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var logDir = Path.Combine(appDataPath, "ArkTweaks", "Logs");
            var exportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ArkTweaks_Logs_Export");
            
            if (Directory.Exists(logDir))
            {
                DirectoryCopy(logDir, exportPath);
                Process.Start(new ProcessStartInfo
                {
                    FileName = exportPath,
                    UseShellExecute = true
                });
                Logger.LogInformation("Logs exported to {Path}", exportPath);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to export logs");
        }
    }

    public void ResetToDefaults()
    {
        _settingsService.ResetToDefaults();
        _settings = _settingsService.CurrentSettings;
        OnPropertyChanged(nameof(LaunchOnStartup));
        OnPropertyChanged(nameof(AutoRefreshDashboard));
        OnPropertyChanged(nameof(CheckForUpdates));
        OnPropertyChanged(nameof(Theme));
        OnPropertyChanged(nameof(AccentColor));
        OnPropertyChanged(nameof(AutoCreateRestorePoint));
        OnPropertyChanged(nameof(AutoBackupRegistry));
        OnPropertyChanged(nameof(ShowConfirmationDialogs));
        OnPropertyChanged(nameof(EnableLogging));
        OnPropertyChanged(nameof(LogRetentionDays));
    }

    private void DirectoryCopy(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);
        
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)));
        }
        
        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            DirectoryCopy(dir, Path.Combine(destDir, Path.GetFileName(dir)));
        }
    }
}
