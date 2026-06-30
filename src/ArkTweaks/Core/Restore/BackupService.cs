using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace ArkTweaks.Core.Restore;

public class BackupService
{
    private readonly ILogger<BackupService> _logger;
    private readonly string _backupPath;

    public BackupService(ILogger<BackupService> logger)
    {
        _logger = logger;
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var backupDir = Path.Combine(appDataPath, "ArkTweaks", "Backups");
        Directory.CreateDirectory(backupDir);
        _backupPath = backupDir;
    }

    public bool CreateRegistryBackup(string registryKeyPath, string tweakId)
    {
        try
        {
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryKeyPath);
            if (key == null)
            {
                _logger.LogWarning("Registry key not found for backup: {Key}", registryKeyPath);
                return false;
            }

            var backupFile = Path.Combine(_backupPath, $"{tweakId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.reg");
            ExportRegistryKey(registryKeyPath, backupFile);
            
            _logger.LogInformation("Created registry backup for {TweakId} at {Path}", tweakId, backupFile);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create registry backup for {TweakId}", tweakId);
            return false;
        }
    }

    public bool RestoreRegistryBackup(string backupFile)
    {
        try
        {
            if (!File.Exists(backupFile))
            {
                _logger.LogWarning("Backup file not found: {File}", backupFile);
                return false;
            }

            // Import registry file
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "regedit.exe",
                Arguments = $"/s \"{backupFile}\"",
                UseShellExecute = true,
                Verb = "runas"
            };

            System.Diagnostics.Process.Start(psi)?.WaitForExit();
            
            _logger.LogInformation("Restored registry backup from {File}", backupFile);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore registry backup from {File}", backupFile);
            return false;
        }
    }

    private void ExportRegistryKey(string keyPath, string outputFile)
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "reg.exe",
            Arguments = $"export \"{keyPath}\" \"{outputFile}\" /y",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
        };

        var process = System.Diagnostics.Process.Start(psi);
        process?.WaitForExit();
    }

    public string GetBackupPath()
    {
        return _backupPath;
    }

    public string[] GetBackupFiles()
    {
        if (!Directory.Exists(_backupPath))
        {
            return Array.Empty<string>();
        }

        return Directory.GetFiles(_backupPath, "*.reg");
    }

    public bool DeleteBackup(string backupFile)
    {
        try
        {
            if (File.Exists(backupFile))
            {
                File.Delete(backupFile);
                _logger.LogInformation("Deleted backup file: {File}", backupFile);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete backup file: {File}", backupFile);
            return false;
        }
    }
}
