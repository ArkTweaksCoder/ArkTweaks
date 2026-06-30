namespace ArkTweaks.Core.Registry;

/// <summary>
/// Result of a registry operation.
/// </summary>
public class RegistryOperationResult
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The registry key path that was operated on
    /// </summary>
    public string KeyPath { get; set; } = string.Empty;

    /// <summary>
    /// The value name that was operated on
    /// </summary>
    public string ValueName { get; set; } = string.Empty;

    /// <summary>
    /// The operation type that was performed
    /// </summary>
    public RegistryOperationType OperationType { get; set; }

    /// <summary>
    /// Timestamp of the operation
    /// </summary>
    public System.DateTime Timestamp { get; set; } = System.DateTime.UtcNow;
}

/// <summary>
/// Types of registry operations.
/// </summary>
public enum RegistryOperationType
{
    Read,
    Write,
    Delete,
    CreateKey,
    DeleteKey
}
