using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using ArkTweaks.Models;

namespace ArkTweaks.Services;

public class SystemInfoService
{
    private readonly ILogger<SystemInfoService> _logger;

    public SystemInfoService(ILogger<SystemInfoService> logger)
    {
        _logger = logger;
    }

    public SystemInfo GetSystemInfo()
    {
        var info = new SystemInfo();

        try
        {
            // CPU Usage (simplified for MVP)
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // First call returns 0
            System.Threading.Thread.Sleep(1000); // Wait for accurate reading
            info.CpuUsage = (int)cpuCounter.NextValue();
            cpuCounter.Dispose();

            // RAM Usage
            var memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            info.RamUsage = (int)memCounter.NextValue();
            memCounter.Dispose();

            // Storage Usage (C: drive)
            var drive = new DriveInfo("C:\\");
            if (drive.IsReady)
            {
                var freeSpace = drive.AvailableFreeSpace;
                var totalSpace = drive.TotalSize;
                info.StorageUsage = (int)(((totalSpace - freeSpace) * 100) / totalSpace);
            }

            // OS Version
            info.OsVersion = Environment.OSVersion.VersionString;

            // Processor Name
            info.ProcessorName = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "Unknown";

            // Memory info
            var memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(ref memStatus))
            {
                info.TotalMemory = (long)memStatus.ullTotalPhys;
                info.AvailableMemory = (long)memStatus.ullAvailPhys;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get system info");
            
            // Return default values on error
            info.CpuUsage = 0;
            info.RamUsage = 0;
            info.StorageUsage = 0;
        }

        return info;
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX()
        {
            dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }
}
