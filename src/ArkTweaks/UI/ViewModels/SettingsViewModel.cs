using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly LicenseService _licenseService;
    private string _licenseKeyInput = string.Empty;

    public string LicenseKeyInput
    {
        get => _licenseKeyInput;
        set => SetProperty(ref _licenseKeyInput, value);
    }

    public Models.LicenseTier CurrentTier => _licenseService.CurrentLicense.Tier;
    public string CurrentLicenseKey => _licenseService.CurrentLicense.LicenseKey;

    public SettingsViewModel(ILogger<SettingsViewModel> logger, LicenseService licenseService) 
        : base(logger)
    {
        _licenseService = licenseService;
    }

    public bool ActivateLicense()
    {
        if (string.IsNullOrWhiteSpace(LicenseKeyInput))
            return false;

        return _licenseService.ActivateLicense(LicenseKeyInput);
    }

    public void DeactivateLicense()
    {
        _licenseService.DeactivateLicense();
    }
}
