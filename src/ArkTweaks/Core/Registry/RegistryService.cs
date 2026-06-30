using System;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ArkTweaks.Core.Safety;
using Microsoft.Extensions.DependencyInjection;

namespace ArkTweaks.Core.Registry;

/// <summary>
/// Service for safe registry operations with validation, backup, logging, and rollback support.
/// </summary>
public class RegistryService
{
    private readonly ILogger<RegistryService> _logger;
    private readonly SafetyValidator _safetyValidator;
    private readonly RegistryBackupService _backupService;
    private readonly RegistryRestoreService _restoreService;
    private readonly IServiceProvider _serviceProvider;

    public RegistryService(
        ILogger<RegistryService> logger,
        SafetyValidator safetyValidator,
        RegistryBackupService backupService,
        RegistryRestoreService restoreService,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _safetyValidator = safetyValidator;
        _backupService = backupService;
        _restoreService = restoreService;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Reads a registry value.
    /// </summary>
    public RegistryOperationResult ReadValue(
        RegistryLocation location,
        string keyPath,
        string valueName)
    {
        var result = new RegistryOperationResult
        {
            KeyPath = keyPath,
            ValueName = valueName,
            OperationType = RegistryOperationType.Read
        };

        try
        {
            var hive = location.ToRegistryHive();
            using var key = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var subKey = key.OpenSubKey(keyPath);

            if (subKey == null)
            {
                result.Success = false;
                result.ErrorMessage = $"Registry key not found: {keyPath}";
                return result;
            }

            var value = subKey.GetValue(valueName);
            result.Success = true;

            _logger.LogDebug("Read registry value {Key}\\{Value}", keyPath, valueName);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error reading registry value {Key}\\{Value}", keyPath, valueName);
        }

        return result;
    }

    /// <summary>
    /// Writes a registry value with validation, backup, and logging.
    /// </summary>
    public RegistryOperationResult WriteValue(
        RegistryLocation location,
        string keyPath,
        string valueName,
        object value,
        RegistryValueKind valueKind,
        bool createBackup = true,
        bool validate = true)
    {
        var result = new RegistryOperationResult
        {
            KeyPath = keyPath,
            ValueName = valueName,
            OperationType = RegistryOperationType.Write
        };

        try
        {
            // Validate if requested
            if (validate)
            {
                var validation = _safetyValidator.ValidateAction(
                    $"Write Registry: {keyPath}\\{valueName}",
                    RiskLevel.Medium,
                    "Registry modification"
                );

                if (!validation.IsValid)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Safety validation failed: {validation.Reason}";
                    _logger.LogWarning("Registry write blocked by safety validator: {Reason}", validation.Reason);
                    return result;
                }
            }

            // Create backup if requested
            string? backupFile = null;
            if (createBackup)
            {
                backupFile = _backupService.BackupKey(location, keyPath);
                if (string.IsNullOrEmpty(backupFile))
                {
                    _logger.LogWarning("Failed to create backup for registry key: {KeyPath}", keyPath);
                }
            }

            // Perform the write
            var hive = location.ToRegistryHive();
            using var key = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var subKey = key.CreateSubKey(keyPath, true);

            var regValueKind = ConvertToRegistryValueKind(valueKind);
            subKey.SetValue(valueName, value, regValueKind);

            result.Success = true;
            _logger.LogInformation("Wrote registry value {Key}\\{Value} = {Data}", keyPath, valueName, value);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error writing registry value {Key}\\{Value}", keyPath, valueName);
        }

        return result;
    }

    /// <summary>
    /// Deletes a registry value with backup and logging.
    /// </summary>
    public RegistryOperationResult DeleteValue(
        RegistryLocation location,
        string keyPath,
        string valueName,
        bool createBackup = true)
    {
        var result = new RegistryOperationResult
        {
            KeyPath = keyPath,
            ValueName = valueName,
            OperationType = RegistryOperationType.Delete
        };

        try
        {
            // Create backup if requested
            if (createBackup)
            {
                var backupFile = _backupService.BackupKey(location, keyPath);
                if (string.IsNullOrEmpty(backupFile))
                {
                    _logger.LogWarning("Failed to create backup for registry key: {KeyPath}", keyPath);
                }
            }

            // Perform the delete
            var hive = location.ToRegistryHive();
            using var key = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var subKey = key.OpenSubKey(keyPath, true);

            if (subKey != null)
            {
                subKey.DeleteValue(valueName, false);
                result.Success = true;
                _logger.LogInformation("Deleted registry value {Key}\\{Value}", keyPath, valueName);
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = $"Registry key not found: {keyPath}";
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error deleting registry value {Key}\\{Value}", keyPath, valueName);
        }

        return result;
    }

    /// <summary>
    /// Creates a registry key.
    /// </summary>
    public RegistryOperationResult CreateKey(
        RegistryLocation location,
        string keyPath)
    {
        var result = new RegistryOperationResult
        {
            KeyPath = keyPath,
            OperationType = RegistryOperationType.CreateKey
        };

        try
        {
            var hive = location.ToRegistryHive();
            using var key = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            key.CreateSubKey(keyPath);

            result.Success = true;
            _logger.LogInformation("Created registry key: {Key}", keyPath);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error creating registry key: {Key}", keyPath);
        }

        return result;
    }

    /// <summary>
    /// Deletes a registry key with backup.
    /// </summary>
    public RegistryOperationResult DeleteKey(
        RegistryLocation location,
        string keyPath,
        bool createBackup = true)
    {
        var result = new RegistryOperationResult
        {
            KeyPath = keyPath,
            OperationType = RegistryOperationType.DeleteKey
        };

        try
        {
            // Create backup if requested
            if (createBackup)
            {
                var backupFile = _backupService.BackupKey(location, keyPath);
                if (string.IsNullOrEmpty(backupFile))
                {
                    _logger.LogWarning("Failed to create backup for registry key: {KeyPath}", keyPath);
                }
            }

            // Perform the delete
            var hive = location.ToRegistryHive();
            using var key = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            key.DeleteSubKeyTree(keyPath);

            result.Success = true;
            _logger.LogInformation("Deleted registry key: {Key}", keyPath);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error deleting registry key: {Key}", keyPath);
        }

        return result;
    }

    /// <summary>
    /// Creates a transaction for atomic registry operations.
    /// </summary>
    public RegistryTransaction CreateTransaction()
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<RegistryTransaction>>();
        var actionLogger = _serviceProvider.GetRequiredService<Core.Logging.ActionLogger>();
        return new RegistryTransaction(logger, actionLogger);
    }

    private Microsoft.Win32.RegistryValueKind ConvertToRegistryValueKind(RegistryValueKind kind)
    {
        return kind switch
        {
            RegistryValueKind.String => Microsoft.Win32.RegistryValueKind.String,
            RegistryValueKind.ExpandString => Microsoft.Win32.RegistryValueKind.ExpandString,
            RegistryValueKind.DWord => Microsoft.Win32.RegistryValueKind.DWord,
            RegistryValueKind.QWord => Microsoft.Win32.RegistryValueKind.QWord,
            RegistryValueKind.MultiString => Microsoft.Win32.RegistryValueKind.MultiString,
            RegistryValueKind.Binary => Microsoft.Win32.RegistryValueKind.Binary,
            RegistryValueKind.None => Microsoft.Win32.RegistryValueKind.None,
            _ => Microsoft.Win32.RegistryValueKind.String
        };
    }
}
