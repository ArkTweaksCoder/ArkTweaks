using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class PerformanceViewModel : BaseViewModel
{
    private readonly LicenseService _licenseService;

    // Stub for diagnostics features - will be expanded in future phases
    public bool HasDiagnostics => _licenseService.IsFeatureEnabled(LicenseFeature.Diagnostics);

    public PerformanceViewModel(ILogger<PerformanceViewModel> logger, LicenseService licenseService) 
        : base(logger)
    {
        _licenseService = licenseService;
    }
}
