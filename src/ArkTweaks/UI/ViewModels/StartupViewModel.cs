using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class StartupViewModel : BaseViewModel
{
    private readonly StartupService _startupService;
    private readonly LicenseService _licenseService;

    public bool CanManage => _licenseService.IsFeatureEnabled(LicenseFeature.StartupManager);

    public StartupViewModel(ILogger<StartupViewModel> logger, StartupService startupService, LicenseService licenseService) 
        : base(logger)
    {
        _startupService = startupService;
        _licenseService = licenseService;
    }
}
