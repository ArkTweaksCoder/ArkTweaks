namespace ArkTweaks.Core.Registry;

/// <summary>
/// Represents a registry value with its data and type.
/// </summary>
public class RegistryValue
{
    /// <summary>
    /// Name of the registry value (empty string for default value)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The value data
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Registry value kind (DWORD, String, etc.)
    /// </summary>
    public RegistryValueKind ValueKind { get; set; }

    /// <summary>
    /// Original value before modification (for rollback)
    /// </summary>
    public object? OriginalData { get; set; }

    /// <summary>
    /// Whether this value was modified
    /// </summary>
    public bool WasModified { get; set; }
}

/// <summary>
/// Registry value kind enumeration.
/// </summary>
public enum RegistryValueKind
{
    String,
    ExpandString,
    DWord,
    QWord,
    MultiString,
    Binary,
    None
}
