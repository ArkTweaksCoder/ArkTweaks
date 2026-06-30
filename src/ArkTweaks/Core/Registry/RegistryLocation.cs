using Microsoft.Win32;

namespace ArkTweaks.Core.Registry;

/// <summary>
/// Registry hive locations for registry operations.
/// </summary>
public enum RegistryLocation
{
    /// <summary>
    /// HKEY_CLASSES_ROOT
    /// </summary>
    ClassesRoot,

    /// <summary>
    /// HKEY_CURRENT_USER
    /// </summary>
    CurrentUser,

    /// <summary>
    /// HKEY_LOCAL_MACHINE
    /// </summary>
    LocalMachine,

    /// <summary>
    /// HKEY_USERS
    /// </summary>
    Users,

    /// <summary>
    /// HKEY_CURRENT_CONFIG
    /// </summary>
    CurrentConfig
}

/// <summary>
/// Extension methods for RegistryLocation.
/// </summary>
public static class RegistryLocationExtensions
{
    /// <summary>
    /// Converts RegistryLocation to RegistryHive.
    /// </summary>
    public static RegistryHive ToRegistryHive(this RegistryLocation location)
    {
        return location switch
        {
            RegistryLocation.ClassesRoot => RegistryHive.ClassesRoot,
            RegistryLocation.CurrentUser => RegistryHive.CurrentUser,
            RegistryLocation.LocalMachine => RegistryHive.LocalMachine,
            RegistryLocation.Users => RegistryHive.Users,
            RegistryLocation.CurrentConfig => RegistryHive.CurrentConfig,
            _ => RegistryHive.CurrentUser
        };
    }
}
