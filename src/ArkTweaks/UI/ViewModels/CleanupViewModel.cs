using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Core.Engine;

namespace ArkTweaks.UI.ViewModels;

public class CleanupViewModel : BaseViewModel
{
    private readonly TempCleanerService _tempCleanerService;
    private readonly LicenseService _licenseService;
    private readonly OptimizationEngine _optimizationEngine;
    private string _tempSize = "0 MB";
    private string _recycleBinSize = "0 MB";
    private bool _isCleaning;
    private string _cleanStatus = "Ready";

    public string TempSize
    {
        get => _tempSize;
        set => SetProperty(ref _tempSize, value);
    }

    public string RecycleBinSize
    {
        get => _recycleBinSize;
        set => SetProperty(ref _recycleBinSize, value);
    }

    public bool IsCleaning
    {
        get => _isCleaning;
        set => SetProperty(ref _isCleaning, value);
    }

    public string CleanStatus
    {
        get => _cleanStatus;
        set => SetProperty(ref _cleanStatus, value);
    }

    public bool CanCleanTemp => _licenseService.IsFeatureEnabled(LicenseFeature.TempCleanup);
    public bool CanCleanRecycleBin => _licenseService.IsFeatureEnabled(LicenseFeature.RecycleBinCleanup);

    public CleanupViewModel(
        ILogger<CleanupViewModel> logger,
        TempCleanerService tempCleanerService,
        LicenseService licenseService,
        OptimizationEngine optimizationEngine) 
        : base(logger)
    {
        _tempCleanerService = tempCleanerService;
        _licenseService = licenseService;
        _optimizationEngine = optimizationEngine;
        ScanTempFiles();
    }

    private void ScanTempFiles()
    {
        try
        {
            var size = _tempCleanerService.ScanTempFiles();
            TempSize = $"{size / 1024 / 1024} MB";
            
            // Estimate recycle bin size (simplified)
            RecycleBinSize = "Unknown";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to scan temp files");
        }
    }

    public async Task CleanTempFilesAsync()
    {
        if (!CanCleanTemp) return;

        IsCleaning = true;
        CleanStatus = "Cleaning temporary files...";
        try
        {
            var result = await _optimizationEngine.ExecuteTweakAsync("cleanup_temp_files");
            if (result.Success)
            {
                TempSize = "0 MB";
                CleanStatus = "Temporary files cleaned successfully";
            }
            else
            {
                CleanStatus = $"Failed: {result.ErrorMessage}";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to clean temp files");
            CleanStatus = $"Error: {ex.Message}";
        }
        finally
        {
            IsCleaning = false;
        }
    }

    public async Task CleanRecycleBinAsync()
    {
        if (!CanCleanRecycleBin) return;

        IsCleaning = true;
        CleanStatus = "Emptying recycle bin...";
        try
        {
            var result = await _optimizationEngine.ExecuteTweakAsync("cleanup_recycle_bin");
            if (result.Success)
            {
                RecycleBinSize = "0 MB";
                CleanStatus = "Recycle bin emptied successfully";
            }
            else
            {
                CleanStatus = $"Failed: {result.ErrorMessage}";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to empty recycle bin");
            CleanStatus = $"Error: {ex.Message}";
        }
        finally
        {
            IsCleaning = false;
        }
    }

    public void Refresh()
    {
        ScanTempFiles();
        CleanStatus = "Ready";
    }
}
