using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class GamingViewModel : BaseViewModel
{
    private readonly LicenseService _licenseService;

    // Stub for gaming features - will be expanded in future phases
    public bool HasGamingProfiles => _licenseService.IsFeatureEnabled(LicenseFeature.GamingProfiles);

    public GamingViewModel(ILogger<GamingViewModel> logger, LicenseService licenseService) 
        : base(logger)
    {
        _licenseService = licenseService;
    }
}
