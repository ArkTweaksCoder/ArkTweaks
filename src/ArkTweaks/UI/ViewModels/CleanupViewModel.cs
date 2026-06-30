using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class CleanupViewModel : BaseViewModel
{
    private readonly TempCleanerService _tempCleanerService;
    private readonly LicenseService _licenseService;
    private string _tempSize = "0 MB";
    private bool _isCleaning;

    public string TempSize
    {
        get => _tempSize;
        set => SetProperty(ref _tempSize, value);
    }

    public bool IsCleaning
    {
        get => _isCleaning;
        set => SetProperty(ref _isCleaning, value);
    }

    public bool CanClean => _licenseService.IsFeatureEnabled(LicenseFeature.TempCleanup);

    public CleanupViewModel(ILogger<CleanupViewModel> logger, TempCleanerService tempCleanerService, LicenseService licenseService) 
        : base(logger)
    {
        _tempCleanerService = tempCleanerService;
        _licenseService = licenseService;
        ScanTempFiles();
    }

    private void ScanTempFiles()
    {
        try
        {
            var size = _tempCleanerService.ScanTempFiles();
            TempSize = $"{size / 1024 / 1024} MB";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to scan temp files");
        }
    }

    public async Task CleanTempFilesAsync()
    {
        if (!CanClean) return;

        IsCleaning = true;
        try
        {
            await Task.Run(() => _tempCleanerService.CleanTempFiles());
            TempSize = "0 MB";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to clean temp files");
        }
        finally
        {
            IsCleaning = false;
        }
    }
}
