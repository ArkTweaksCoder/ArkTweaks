using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Services;

public class TempCleanerService
{
    private readonly ILogger<TempCleanerService> _logger;

    public TempCleanerService(ILogger<TempCleanerService> logger)
    {
        _logger = logger;
    }

    public class CleanupResult
    {
        public bool Success { get; set; }
        public long SpaceFreed { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public long ScanTempFiles()
    {
        long totalSize = 0;

        try
        {
            var tempPath = Path.GetTempPath();
            
            if (Directory.Exists(tempPath))
            {
                totalSize += GetDirectorySize(tempPath);
            }

            var windowsTemp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
            
            if (Directory.Exists(windowsTemp))
            {
                totalSize += GetDirectorySize(windowsTemp);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scan temp files");
        }

        return totalSize;
    }

    public void CleanTempFiles()
    {
        try
        {
            var tempPath = Path.GetTempPath();
            
            if (Directory.Exists(tempPath))
            {
                CleanDirectory(tempPath);
            }

            var windowsTemp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
            
            if (Directory.Exists(windowsTemp))
            {
                CleanDirectory(windowsTemp);
            }

            _logger.LogInformation("Temp files cleaned successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clean temp files");
            throw;
        }
    }

    public async Task<CleanupResult> CleanupTempFilesAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                var sizeBefore = ScanTempFiles();
                CleanTempFiles();
                var sizeAfter = ScanTempFiles();
                
                return new CleanupResult
                {
                    Success = true,
                    SpaceFreed = sizeBefore - sizeAfter
                };
            }
            catch (Exception ex)
            {
                return new CleanupResult
                {
                    Success = false,
                    SpaceFreed = 0,
                    ErrorMessage = ex.Message
                };
            }
        });
    }

    private long GetDirectorySize(string path)
    {
        long size = 0;

        try
        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                try
                {
                    size += new FileInfo(file).Length;
                }
                catch
                {
                    // Skip files that can't be accessed
                }
            }
        }
        catch
        {
            // Skip directories that can't be accessed
        }

        return size;
    }

    private void CleanDirectory(string path)
    {
        try
        {
            var files = Directory.GetFiles(path);
            
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // Skip locked files
                }
            }

            var directories = Directory.GetDirectories(path);
            
            foreach (var dir in directories)
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch
                {
                    // Skip directories that can't be deleted
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to clean directory: {Path}", path);
        }
    }
}
