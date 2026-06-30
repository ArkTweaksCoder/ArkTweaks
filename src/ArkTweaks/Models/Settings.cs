namespace ArkTweaks.Models;

/// <summary>
/// Application settings with all categories.
/// </summary>
public class Settings
{
    // General Settings
    public bool LaunchOnStartup { get; set; } = false;
    public bool AutoRefreshDashboard { get; set; } = true;
    public bool CheckForUpdates { get; set; } = true;

    // Appearance Settings
    public AppTheme Theme { get; set; } = AppTheme.Dark;
    public string AccentColor { get; set; } = "#3B82F6";

    // Optimization Settings
    public bool AutoCreateRestorePoint { get; set; } = true;
    public bool AutoBackupRegistry { get; set; } = true;
    public bool ShowConfirmationDialogs { get; set; } = true;

    // Logs Settings
    public bool EnableLogging { get; set; } = true;
    public int LogRetentionDays { get; set; } = 30;

    // License Settings (stored separately, but included for reference)
    public string LicenseKey { get; set; } = string.Empty;
    public LicenseTier CurrentTier { get; set; } = LicenseTier.Free;

    // About Settings (read-only, stored for display)
    public string Version { get; set; } = "1.0.0";
    public string ApplicationName { get; set; } = "Ark Tweaks";
}

/// <summary>
/// Application theme options.
/// </summary>
public enum AppTheme
{
    Light,
    Dark,
    System
}
