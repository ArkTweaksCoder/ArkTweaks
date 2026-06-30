using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace ArkTweaks.Core.Registry;

/// <summary>
/// Service for creating and managing registry backups.
/// </summary>
public class RegistryBackupService
{
    private readonly ILogger<RegistryBackupService> _logger;
    private readonly string _backupDirectory;

    public RegistryBackupService(ILogger<RegistryBackupService> logger)
    {
        _logger = logger;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _backupDirectory = Path.Combine(appDataPath, "ArkTweaks", "RegistryBackups");
        Directory.CreateDirectory(_backupDirectory);
    }

    /// <summary>
    /// Creates a backup of a registry key.
    /// </summary>
    public string BackupKey(RegistryLocation location, string keyPath)
    {
        var backupId = Guid.NewGuid().ToString();
        var backupFile = Path.Combine(_backupDirectory, $"{backupId}.reg");

        try
        {
            var hive = location.ToRegistryHive();
            var fullKeyPath = $"{GetHiveName(hive)}\\{keyPath}";

            // Use reg.exe to export the key
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = $"export \"{fullKeyPath}\" \"{backupFile}\" /y",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            process?.WaitForExit();

            if (process?.ExitCode == 0 && File.Exists(backupFile))
            {
                _logger.LogInformation("Created registry backup: {BackupFile}", backupFile);
                return backupFile;
            }
            else
            {
                _logger.LogError("Failed to create registry backup for {KeyPath}", fullKeyPath);
                File.Delete(backupFile);
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating registry backup for {KeyPath}", keyPath);
            if (File.Exists(backupFile))
            {
                File.Delete(backupFile);
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Creates a backup of a specific registry value.
    /// </summary>
    public string BackupValue(RegistryLocation location, string keyPath, string valueName)
    {
        // For individual values, we backup the entire key
        return BackupKey(location, keyPath);
    }

    /// <summary>
    /// Deletes old backups older than specified days.
    /// </summary>
    public void CleanupOldBackups(int daysToKeep = 30)
    {
        try
        {
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
            var files = Directory.GetFiles(_backupDirectory, "*.reg");

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < cutoffDate)
                {
                    File.Delete(file);
                    _logger.LogDebug("Deleted old backup: {File}", file);
                }
            }

            _logger.LogInformation("Cleaned up old registry backups");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old backups");
        }
    }

    /// <summary>
    /// Gets the hive name for the registry hive.
    /// </summary>
    private string GetHiveName(RegistryHive hive)
    {
        return hive switch
        {
            RegistryHive.ClassesRoot => "HKEY_CLASSES_ROOT",
            RegistryHive.CurrentUser => "HKEY_CURRENT_USER",
            RegistryHive.LocalMachine => "HKEY_LOCAL_MACHINE",
            RegistryHive.Users => "HKEY_USERS",
            RegistryHive.CurrentConfig => "HKEY_CURRENT_CONFIG",
            _ => "HKEY_CURRENT_USER"
        };
    }
}
