using System;
using System.Threading.Tasks;
using ArkTweaks.Core.Safety;
using ArkTweaks.Core.Tweaks;
using ArkTweaks.Models;
using ArkTweaks.Services;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Tweaks.Power;

public class HighPerformancePowerPlanTweak : BaseTweak
{
    private readonly PowerPlanService _powerPlanService;
    private string? _previousPlanGuid;

    public HighPerformancePowerPlanTweak(ILogger<HighPerformancePowerPlanTweak> logger, PowerPlanService powerPlanService)
        : base(logger)
    {
        _powerPlanService = powerPlanService;
    }

    public override string Id => "power_high_performance";
    public override string Name => "High Performance Power Plan";
    public override string Description => "Switches the system to the High Performance power plan for maximum performance.";
    public override TweakCategory Category => TweakCategory.Power;
    public override LicenseTier RequiredLicenseTier => LicenseTier.Free;
    public override RiskLevel RiskLevel => RiskLevel.Low;
    public override bool IsReversible => true;

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            Logger.LogInformation("Switching to High Performance power plan");
            
            // Save current plan for reversion
            var currentPlan = await _powerPlanService.GetCurrentPowerPlanAsync();
            _previousPlanGuid = currentPlan?.Guid.ToString();
            
            var result = await _powerPlanService.SetPowerPlanAsync("High Performance");
            
            if (result)
            {
                Logger.LogInformation("Successfully switched to High Performance power plan");
                return CreateResult(true, "Switched to High Performance power plan");
            }
            else
            {
                Logger.LogError("Failed to switch to High Performance power plan");
                return CreateResult(false, "Failed to switch power plan", "High Performance plan may not be available on this system");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error switching power plan");
            return CreateResult(false, "Failed to switch power plan", ex.Message);
        }
    }

    public override async Task<TweakResult> RevertAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_previousPlanGuid))
            {
                Logger.LogWarning("No previous power plan saved for reversion");
                return CreateResult(false, "Cannot revert", "No previous power plan was saved");
            }

            Logger.LogInformation("Reverting to previous power plan: {Guid}", _previousPlanGuid);
            
            var result = await _powerPlanService.SetPowerPlanByGuidAsync(_previousPlanGuid);
            
            if (result)
            {
                Logger.LogInformation("Successfully reverted to previous power plan");
                return CreateResult(true, "Reverted to previous power plan");
            }
            else
            {
                Logger.LogError("Failed to revert power plan");
                return CreateResult(false, "Failed to revert power plan", "Could not restore previous power plan");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reverting power plan");
            return CreateResult(false, "Failed to revert power plan", ex.Message);
        }
    }
}
