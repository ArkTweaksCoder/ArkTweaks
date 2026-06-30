using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly LicenseService _licenseService;

    public LicenseService LicenseService => _licenseService;

    public MainViewModel(ILogger<MainViewModel> logger, LicenseService licenseService) 
        : base(logger)
    {
        _licenseService = licenseService;
    }
}
