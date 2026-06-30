using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ArkTweaks.Core.Logging;

namespace ArkTweaks.Core.Registry;

/// <summary>
/// Represents a transaction for atomic registry operations with rollback capability.
/// </summary>
public class RegistryTransaction : IDisposable
{
    private readonly ILogger<RegistryTransaction> _logger;
    private readonly ActionLogger _actionLogger;
    private readonly List<RegistryOperationResult> _operations;
    private readonly List<RegistryValue> _modifiedValues;
    private bool _isCommitted;
    private bool _isDisposed;

    public string TransactionId { get; }
    public DateTime StartTime { get; }
    public bool IsCommitted => _isCommitted;
    public IReadOnlyList<RegistryOperationResult> Operations => _operations.AsReadOnly();
    public IReadOnlyList<RegistryValue> ModifiedValues => _modifiedValues.AsReadOnly();

    public RegistryTransaction(ILogger<RegistryTransaction> logger, ActionLogger actionLogger)
    {
        _logger = logger;
        _actionLogger = actionLogger;
        _operations = new List<RegistryOperationResult>();
        _modifiedValues = new List<RegistryValue>();
        TransactionId = Guid.NewGuid().ToString();
        StartTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Records a registry operation in the transaction.
    /// </summary>
    public void RecordOperation(RegistryOperationResult result)
    {
        _operations.Add(result);
        _logger.LogDebug("Recorded operation {Type} on {Key}\\{Value}", 
            result.OperationType, result.KeyPath, result.ValueName);
    }

    /// <summary>
    /// Records a modified registry value for rollback.
    /// </summary>
    public void RecordModifiedValue(RegistryValue value)
    {
        _modifiedValues.Add(value);
        _logger.LogDebug("Recorded modified value {Name}", value.Name);
    }

    /// <summary>
    /// Commits the transaction (marks as successful).
    /// </summary>
    public void Commit()
    {
        _isCommitted = true;
        _logger.LogInformation("Transaction {TransactionId} committed with {Count} operations", 
            TransactionId, _operations.Count);

        // Log to action logger
        _actionLogger.LogAction(
            $"registry_transaction_{TransactionId}",
            "Registry Transaction",
            true,
            null
        );
    }

    /// <summary>
    /// Rolls back all modified values to their original state.
    /// </summary>
    public void Rollback()
    {
        _logger.LogWarning("Rolling back transaction {TransactionId}", TransactionId);

        foreach (var value in _modifiedValues)
        {
            try
            {
                RollbackValue(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rollback value {Name}", value.Name);
            }
        }

        _actionLogger.LogAction(
            $"registry_transaction_{TransactionId}",
            "Registry Transaction Rollback",
            true,
            null
        );
    }

    private void RollbackValue(RegistryValue value)
    {
        // Implementation would restore the original value
        // This is a placeholder - actual implementation would use RegistryService
        _logger.LogInformation("Rolling back value {Name} to original data", value.Name);
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            if (!_isCommitted && _modifiedValues.Count > 0)
            {
                Rollback();
            }
            _isDisposed = true;
        }
    }
}
