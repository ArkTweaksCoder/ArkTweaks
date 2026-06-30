using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Core.Registry;

/// <summary>
/// Service for restoring registry backups.
/// </summary>
public class RegistryRestoreService
{
    private readonly ILogger<RegistryRestoreService> _logger;
    private readonly string _backupDirectory;

    public RegistryRestoreService(ILogger<RegistryRestoreService> logger)
    {
        _logger = logger;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _backupDirectory = Path.Combine(appDataPath, "ArkTweaks", "RegistryBackups");
    }

    /// <summary>
    /// Restores a registry key from a backup file.
    /// </summary>
    public bool RestoreFromBackup(string backupFilePath)
    {
        if (!File.Exists(backupFilePath))
        {
            _logger.LogError("Backup file not found: {BackupFile}", backupFilePath);
            return false;
        }

        try
        {
            // Use reg.exe to import the backup
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = $"import \"{backupFilePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Verb = "runas" // Requires admin privileges
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            process?.WaitForExit();

            if (process?.ExitCode == 0)
            {
                _logger.LogInformation("Successfully restored registry from backup: {BackupFile}", backupFilePath);
                return true;
            }
            else
            {
                _logger.LogError("Failed to restore registry from backup: {BackupFile}", backupFilePath);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring registry from backup: {BackupFile}", backupFilePath);
            return false;
        }
    }

    /// <summary>
    /// Gets all available backup files.
    /// </summary>
    public string[] GetAvailableBackups()
    {
        try
        {
            if (!Directory.Exists(_backupDirectory))
            {
                return Array.Empty<string>();
            }

            return Directory.GetFiles(_backupDirectory, "*.reg");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available backups");
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Deletes a specific backup file.
    /// </summary>
    public bool DeleteBackup(string backupFilePath)
    {
        try
        {
            if (File.Exists(backupFilePath))
            {
                File.Delete(backupFilePath);
                _logger.LogInformation("Deleted backup: {BackupFile}", backupFilePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting backup: {BackupFile}", backupFilePath);
            return false;
        }
    }
}
