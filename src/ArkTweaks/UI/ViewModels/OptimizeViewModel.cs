using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class OptimizeViewModel : BaseViewModel
{
    private readonly LicenseService _licenseService;

    public OptimizeViewModel(ILogger<OptimizeViewModel> logger, LicenseService licenseService) 
        : base(logger)
    {
        _licenseService = licenseService;
    }

    // Stub for advanced tweaks - will be expanded in future phases
    public bool HasAdvancedFeatures => false;
}
