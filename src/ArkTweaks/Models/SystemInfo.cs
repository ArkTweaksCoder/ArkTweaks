namespace ArkTweaks.Models;

public class SystemInfo
{
    public int CpuUsage { get; set; }
    public int RamUsage { get; set; }
    public int StorageUsage { get; set; }
    public string OsVersion { get; set; } = string.Empty;
    public string ProcessorName { get; set; } = string.Empty;
    public long TotalMemory { get; set; }
    public long AvailableMemory { get; set; }
}
