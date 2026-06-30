using System;
using System.Linq;
using System.Threading.Tasks;
using ArkTweaks.Core.Safety;
using ArkTweaks.Core.Tweaks;
using ArkTweaks.Models;
using ArkTweaks.Services;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Tweaks.Startup;

public class DisableStartupAppTweak : BaseTweak
{
    private readonly StartupService _startupService;
    private readonly string _appName;
    private readonly string _appPath;
    private bool _wasEnabled;

    public DisableStartupAppTweak(
        ILogger<DisableStartupAppTweak> logger,
        StartupService startupService,
        string appName,
        string appPath) : base(logger)
    {
        _startupService = startupService;
        _appName = appName;
        _appPath = appPath;
    }

    public override string Id => $"startup_disable_{_appName.GetHashCode()}";
    public override string Name => $"Disable {_appName} from Startup";
    public override string Description => $"Prevents {_appName} from launching automatically at system startup.";
    public override TweakCategory Category => TweakCategory.Startup;
    public override LicenseTier RequiredLicenseTier => LicenseTier.Free;
    public override RiskLevel RiskLevel => RiskLevel.Low;
    public override bool IsReversible => true;

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            Logger.LogInformation("Disabling startup app: {AppName}", _appName);
            
            // Check if it's currently enabled
            var startupApps = await _startupService.GetStartupAppsAsync();
            var app = startupApps.FirstOrDefault(a => a.Name == _appName && a.Path == _appPath);
            _wasEnabled = app != null && app.Enabled;
            
            if (!_wasEnabled)
            {
                Logger.LogInformation("App {AppName} is already disabled", _appName);
                return CreateResult(true, "App is already disabled from startup");
            }
            
            var result = await _startupService.DisableStartupAppAsync(_appName, _appPath);
            
            if (result)
            {
                Logger.LogInformation("Successfully disabled startup app: {AppName}", _appName);
                return CreateResult(true, $"Disabled {_appName} from startup");
            }
            else
            {
                Logger.LogError("Failed to disable startup app: {AppName}", _appName);
                return CreateResult(false, "Failed to disable app from startup", "Could not modify startup configuration");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error disabling startup app: {AppName}", _appName);
            return CreateResult(false, "Failed to disable app from startup", ex.Message);
        }
    }

    public override async Task<TweakResult> RevertAsync()
    {
        try
        {
            if (!_wasEnabled)
            {
                Logger.LogInformation("App {AppName} was not originally enabled, skipping reversion", _appName);
                return CreateResult(true, "App was not originally enabled");
            }

            Logger.LogInformation("Re-enabling startup app: {AppName}", _appName);
            
            var result = await _startupService.EnableStartupAppAsync(_appName, _appPath);
            
            if (result)
            {
                Logger.LogInformation("Successfully re-enabled startup app: {AppName}", _appName);
                return CreateResult(true, $"Re-enabled {_appName} in startup");
            }
            else
            {
                Logger.LogError("Failed to re-enable startup app: {AppName}", _appName);
                return CreateResult(false, "Failed to re-enable app in startup", "Could not modify startup configuration");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error re-enabling startup app: {AppName}", _appName);
            return CreateResult(false, "Failed to re-enable app in startup", ex.Message);
        }
    }
}
