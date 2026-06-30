using System;
using System.Threading.Tasks;
using ArkTweaks.Core.Safety;
using ArkTweaks.Core.Tweaks;
using ArkTweaks.Models;
using ArkTweaks.Services;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Tweaks.Restore;

public class CreateRestorePointTweak : BaseTweak
{
    private readonly RestorePointService _restorePointService;

    public CreateRestorePointTweak(ILogger<CreateRestorePointTweak> logger, RestorePointService restorePointService)
        : base(logger)
    {
        _restorePointService = restorePointService;
    }

    public override string Id => "restore_create_point";
    public override string Name => "Create System Restore Point";
    public override string Description => "Creates a Windows system restore point before making changes to the system.";
    public override TweakCategory Category => TweakCategory.Restore;
    public override LicenseTier RequiredLicenseTier => LicenseTier.Free;
    public override RiskLevel RiskLevel => RiskLevel.None;
    public override bool IsReversible => false;

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            Logger.LogInformation("Creating system restore point");
            
            var result = await _restorePointService.CreateRestorePointAsync("Ark Tweaks - Before Optimization");
            
            if (result.Success)
            {
                Logger.LogInformation("System restore point created successfully");
                return CreateResult(true, "System restore point created successfully");
            }
            else
            {
                Logger.LogError("Failed to create restore point: {Error}", result.ErrorMessage);
                return CreateResult(false, "Failed to create restore point", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating restore point");
            return CreateResult(false, "Failed to create restore point", ex.Message);
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        return Task.FromResult(CreateResult(false, "This tweak is not reversible", "Restore points cannot be reverted after creation"));
    }
}
