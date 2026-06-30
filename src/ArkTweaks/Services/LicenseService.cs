using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArkTweaks.Models;

namespace ArkTweaks.Services;

public enum LicenseFeature
{
    // Cleanup features
    TempCleanup,
    RecycleBinCleanup,
    BrowserCacheCleanup,
    
    // Startup features
    StartupManager,
    StartupDelayReduction,
    
    // Power features
    PowerPlanSwitch,
    UltimatePerformanceMode,
    
    // Gaming features
    GameMode,
    FullscreenOptimizations,
    GpuScheduling,
    MousePrecisionFix,
    
    // System features
    VisualEffectsOptimization,
    MemoryCleanup,
    SearchIndexOptimization,
    
    // Network features
    DnsSwitcher,
    NetworkReset,
    
    // Advanced features
    Diagnostics,
    Automation,
    GamingProfiles
}

public class LicenseService : INotifyPropertyChanged
{
    private readonly LicenseValidator _validator;
    private readonly LicenseStorage _storage;
    private LicenseInfo _currentLicense;

    public event PropertyChangedEventHandler? PropertyChanged;

    public LicenseInfo CurrentLicense
    {
        get => _currentLicense;
        private set
        {
            _currentLicense = value;
            OnPropertyChanged();
        }
    }

    public LicenseService()
    {
        _validator = new LicenseValidator();
        _storage = new LicenseStorage();
        _currentLicense = new LicenseInfo { Tier = LicenseTier.Free, IsValid = true };
    }

    public void Initialize()
    {
        CurrentLicense = _storage.LoadLicense();
    }

    public bool ActivateLicense(string licenseKey)
    {
        var (isValid, tier, errorMessage) = _validator.ValidateLicenseKey(licenseKey);

        var licenseInfo = new LicenseInfo
        {
            Tier = tier,
            LicenseKey = licenseKey,
            IsValid = isValid,
            ErrorMessage = errorMessage
        };

        if (isValid)
        {
            _storage.SaveLicense(licenseInfo);
            CurrentLicense = licenseInfo;
            return true;
        }

        return false;
    }

    public void DeactivateLicense()
    {
        _storage.ClearLicense();
        CurrentLicense = new LicenseInfo { Tier = LicenseTier.Free, IsValid = true };
    }

    public bool IsFeatureEnabled(LicenseFeature feature)
    {
        return feature switch
        {
            // Free tier features
            LicenseFeature.TempCleanup => true,
            LicenseFeature.StartupManager => true,
            LicenseFeature.PowerPlanSwitch => true,
            LicenseFeature.GameMode => true,
            
            // Standard tier features
            LicenseFeature.RecycleBinCleanup => CurrentLicense.Tier >= LicenseTier.Standard,
            LicenseFeature.StartupDelayReduction => CurrentLicense.Tier >= LicenseTier.Standard,
            LicenseFeature.FullscreenOptimizations => CurrentLicense.Tier >= LicenseTier.Standard,
            LicenseFeature.VisualEffectsOptimization => CurrentLicense.Tier >= LicenseTier.Standard,
            
            // Pro tier features
            LicenseFeature.BrowserCacheCleanup => CurrentLicense.Tier >= LicenseTier.Pro,
            LicenseFeature.UltimatePerformanceMode => CurrentLicense.Tier >= LicenseTier.Pro,
            LicenseFeature.GpuScheduling => CurrentLicense.Tier >= LicenseTier.Pro,
            LicenseFeature.MousePrecisionFix => CurrentLicense.Tier >= LicenseTier.Pro,
            LicenseFeature.MemoryCleanup => CurrentLicense.Tier >= LicenseTier.Pro,
            LicenseFeature.SearchIndexOptimization => CurrentLicense.Tier >= LicenseTier.Pro,
            LicenseFeature.DnsSwitcher => CurrentLicense.Tier >= LicenseTier.Pro,
            LicenseFeature.NetworkReset => CurrentLicense.Tier >= LicenseTier.Pro,
            
            // Ultimate tier features
            LicenseFeature.Diagnostics => CurrentLicense.Tier >= LicenseTier.Ultimate,
            LicenseFeature.Automation => CurrentLicense.Tier >= LicenseTier.Ultimate,
            LicenseFeature.GamingProfiles => CurrentLicense.Tier >= LicenseTier.Ultimate,
            
            _ => false
        };
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
