using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using ArkTweaks.Models;
using ArkTweaks.Core.Safety;

namespace ArkTweaks.Services;

/// <summary>
/// Service for scanning and cleaning various system locations.
/// </summary>
public class CleanupScannerService
{
    private readonly ILogger<CleanupScannerService> _logger;

    public CleanupScannerService(ILogger<CleanupScannerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Scans all cleanup categories.
    /// </summary>
    public List<CleanupCategory> ScanAllCategories()
    {
        var categories = new List<CleanupCategory>();
        
        // System categories
        categories.AddRange(ScanSystemCategories());
        
        // Browser categories
        categories.AddRange(ScanBrowserCategories());
        
        // System Logs categories
        categories.AddRange(ScanSystemLogCategories());
        
        // Storage categories
        categories.AddRange(ScanStorageCategories());

        return categories;
    }

    /// <summary>
    /// Scans a specific category by ID.
    /// </summary>
    public CleanupCategory? ScanCategory(string categoryId)
    {
        return ScanAllCategories().FirstOrDefault(c => c.Id == categoryId);
    }

    /// <summary>
    /// Cleans selected categories.
    /// </summary>
    public CleanupResult CleanCategories(List<CleanupCategory> categories)
    {
        var result = new CleanupResult();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            foreach (var category in categories.Where(c => c.IsSelected))
            {
                var categoryResult = CleanCategory(category);
                result.CategoryResults.Add(categoryResult);
                
                if (categoryResult.Success)
                {
                    result.SpaceReclaimedBytes += categoryResult.SpaceReclaimedBytes;
                    result.FilesDeleted += categoryResult.FilesDeleted;
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error during cleanup");
        }

        stopwatch.Stop();
        result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;

        return result;
    }

    /// <summary>
    /// Cleans a single category.
    /// </summary>
    private CategoryCleanupResult CleanCategory(CleanupCategory category)
    {
        var result = new CategoryCleanupResult
        {
            CategoryId = category.Id,
            CategoryName = category.Name
        };

        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            switch (category.Id)
            {
                case "system_windows_temp":
                    result = CleanDirectory(Path.GetTempPath(), result);
                    break;
                case "system_user_temp":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp"), result);
                    break;
                case "system_update_cache":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution", "Download"), result);
                    break;
                case "system_delivery_optimization":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "ServiceProfiles", "LocalService", "AppData", "Local", "Microsoft", "Windows", "DeliveryOptimization", "Cache"), result);
                    break;
                case "system_thumbnail_cache":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Explorer"), result);
                    break;
                case "system_error_reporting":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "Windows", "WER"), result);
                    break;
                case "system_memory_dumps":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrashDumps"), result);
                    break;
                case "system_directx_cache":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache"), result);
                    break;
                case "browser_edge_cache":
                    result = CleanBrowserCache("Edge", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "User Data", "Default", "Cache"), result);
                    break;
                case "browser_chrome_cache":
                    result = CleanBrowserCache("Chrome", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data", "Default", "Cache"), result);
                    break;
                case "browser_firefox_cache":
                    result = CleanBrowserCache("Firefox", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "Firefox", "Profiles"), result);
                    break;
                case "logs_windows_logs":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Logs"), result, safeOnly: true);
                    break;
                case "storage_recycle_bin":
                    result = EmptyRecycleBin(result);
                    break;
                case "storage_downloaded_program_files":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Downloaded Program Files"), result);
                    break;
                case "storage_temp_installation":
                    result = CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"), result);
                    break;
                default:
                    result.Success = false;
                    result.ErrorMessage = "Unknown category";
                    break;
            }

            stopwatch.Stop();
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error cleaning category {CategoryId}", category.Id);
        }

        return result;
    }

    #region System Categories

    private List<CleanupCategory> ScanSystemCategories()
    {
        var categories = new List<CleanupCategory>();

        categories.Add(new CleanupCategory
        {
            Id = "system_windows_temp",
            Name = "Windows Temp",
            Description = "Temporary files created by Windows",
            CategoryGroup = "System",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.GetTempPath()),
            FileCount = CountFiles(Path.GetTempPath())
        });

        categories.Add(new CleanupCategory
        {
            Id = "system_user_temp",
            Name = "User Temp",
            Description = "Temporary files created by applications",
            CategoryGroup = "System",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp"))
        });

        categories.Add(new CleanupCategory
        {
            Id = "system_update_cache",
            Name = "Windows Update Cache",
            Description = "Cached Windows update files",
            CategoryGroup = "System",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution", "Download")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution", "Download"))
        });

        categories.Add(new CleanupCategory
        {
            Id = "system_delivery_optimization",
            Name = "Delivery Optimization Cache",
            Description = "Windows delivery optimization cache",
            CategoryGroup = "System",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "ServiceProfiles", "LocalService", "AppData", "Local", "Microsoft", "Windows", "DeliveryOptimization", "Cache")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "ServiceProfiles", "LocalService", "AppData", "Local", "Microsoft", "Windows", "DeliveryOptimization", "Cache"))
        });

        categories.Add(new CleanupCategory
        {
            Id = "system_thumbnail_cache",
            Name = "Thumbnail Cache",
            Description = "Windows thumbnail cache",
            CategoryGroup = "System",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Explorer")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Explorer"))
        });

        categories.Add(new CleanupCategory
        {
            Id = "system_error_reporting",
            Name = "Error Reporting Files",
            Description = "Windows Error Reporting files",
            CategoryGroup = "System",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "Windows", "WER")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "Windows", "WER"))
        });

        categories.Add(new CleanupCategory
        {
            Id = "system_memory_dumps",
            Name = "Memory Dump Files",
            Description = "Application crash dump files",
            CategoryGroup = "System",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrashDumps")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrashDumps"))
        });

        categories.Add(new CleanupCategory
        {
            Id = "system_directx_cache",
            Name = "DirectX Shader Cache",
            Description = "DirectX shader cache files",
            CategoryGroup = "System",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache"))
        });

        return categories;
    }

    #endregion

    #region Browser Categories

    private List<CleanupCategory> ScanBrowserCategories()
    {
        var categories = new List<CleanupCategory>();

        categories.Add(new CleanupCategory
        {
            Id = "browser_edge_cache",
            Name = "Microsoft Edge Cache",
            Description = "Edge browser cache files",
            CategoryGroup = "Browsers",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "User Data", "Default", "Cache")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "User Data", "Default", "Cache"))
        });

        categories.Add(new CleanupCategory
        {
            Id = "browser_chrome_cache",
            Name = "Google Chrome Cache",
            Description = "Chrome browser cache files",
            CategoryGroup = "Browsers",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data", "Default", "Cache")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data", "Default", "Cache"))
        });

        categories.Add(new CleanupCategory
        {
            Id = "browser_firefox_cache",
            Name = "Mozilla Firefox Cache",
            Description = "Firefox browser cache files",
            CategoryGroup = "Browsers",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "Firefox", "Profiles")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "Firefox", "Profiles"))
        });

        return categories;
    }

    #endregion

    #region System Log Categories

    private List<CleanupCategory> ScanSystemLogCategories()
    {
        var categories = new List<CleanupCategory>();

        categories.Add(new CleanupCategory
        {
            Id = "logs_windows_logs",
            Name = "Windows Logs",
            Description = "Windows system logs (safe cleanup only)",
            CategoryGroup = "System Logs",
            RiskLevel = RiskLevel.Medium,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Logs")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Logs"))
        });

        return categories;
    }

    #endregion

    #region Storage Categories

    private List<CleanupCategory> ScanStorageCategories()
    {
        var categories = new List<CleanupCategory>();

        categories.Add(new CleanupCategory
        {
            Id = "storage_recycle_bin",
            Name = "Recycle Bin",
            Description = "Deleted files in recycle bin",
            CategoryGroup = "Storage",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = EstimateRecycleBinSize(),
            FileCount = 0
        });

        categories.Add(new CleanupCategory
        {
            Id = "storage_downloaded_program_files",
            Name = "Downloaded Program Files",
            Description = "ActiveX controls and Java applets",
            CategoryGroup = "Storage",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Downloaded Program Files")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Downloaded Program Files"))
        });

        categories.Add(new CleanupCategory
        {
            Id = "storage_temp_installation",
            Name = "Temporary Installation Files",
            Description = "Windows installation temp files",
            CategoryGroup = "Storage",
            RiskLevel = RiskLevel.Low,
            EstimatedSizeBytes = CalculateDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp")),
            FileCount = CountFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"))
        });

        return categories;
    }

    #endregion

    #region Helper Methods

    private long CalculateDirectorySize(string path)
    {
        try
        {
            if (!Directory.Exists(path))
                return 0;

            return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                .Sum(file => new FileInfo(file).Length);
        }
        catch
        {
            return 0;
        }
    }

    private int CountFiles(string path)
    {
        try
        {
            if (!Directory.Exists(path))
                return 0;

            return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Count();
        }
        catch
        {
            return 0;
        }
    }

    private CategoryCleanupResult CleanDirectory(string path, CategoryCleanupResult result, bool safeOnly = false)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                result.Success = true;
                return result;
            }

            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                try
                {
                    // Skip files that are in use or protected
                    var fileInfo = new FileInfo(file);
                    if (safeOnly && IsProtectedFile(file))
                        continue;

                    File.Delete(file);
                    result.SpaceReclaimedBytes += fileInfo.Length;
                    result.FilesDeleted++;
                }
                catch
                {
                    // File in use, skip
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    private CategoryCleanupResult CleanBrowserCache(string browserName, string path, CategoryCleanupResult result)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                result.Success = true;
                return result;
            }

            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    File.Delete(file);
                    result.SpaceReclaimedBytes += fileInfo.Length;
                    result.FilesDeleted++;
                }
                catch
                {
                    // File in use, skip
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    private CategoryCleanupResult EmptyRecycleBin(CategoryCleanupResult result)
    {
        try
        {
            var shellType = Type.GetTypeFromProgID("Shell.Application");
            if (shellType != null)
            {
                var shell = Activator.CreateInstance(shellType);
                if (shell != null)
                {
                    dynamic shellObj = shell;
                    shellObj.Namespace(10).Self.InvokeVerb("Empty Recycle Bin");
                    
                    // Estimate size (simplified)
                    result.SpaceReclaimedBytes = EstimateRecycleBinSize();
                    result.FilesDeleted = 1; // Placeholder
                }
            }
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    private long EstimateRecycleBinSize()
    {
        try
        {
            var recycleBinPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "$Recycle.Bin");
            if (Directory.Exists(recycleBinPath))
            {
                return CalculateDirectorySize(recycleBinPath);
            }
        }
        catch
        {
            // Ignore
        }
        return 0;
    }

    private bool IsProtectedFile(string filePath)
    {
        // Skip protected system files
        var fileName = Path.GetFileName(filePath).ToLower();
        return fileName.Contains(".log") && (filePath.Contains("Windows") || filePath.Contains("System32"));
    }

    #endregion
}
