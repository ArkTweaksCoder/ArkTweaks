using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArkTweaks.Core.Safety;
using ArkTweaks.Core.Tweaks;
using ArkTweaks.Models;
using ArkTweaks.Services;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Tweaks.Storage;

public class TemporaryFilesCleanupTweak : BaseTweak
{
    private readonly TempCleanerService _tempCleaner;

    public TemporaryFilesCleanupTweak(ILogger<TemporaryFilesCleanupTweak> logger, TempCleanerService tempCleaner)
        : base(logger)
    {
        _tempCleaner = tempCleaner;
    }

    public override string Id => "cleanup_temp_files";
    public override string Name => "Temporary Files Cleanup";
    public override string Description => "Cleans Windows temporary files and user temp directories to free disk space.";
    public override TweakCategory Category => TweakCategory.Cleanup;
    public override LicenseTier RequiredLicenseTier => LicenseTier.Free;
    public override RiskLevel RiskLevel => RiskLevel.Low;
    public override bool IsReversible => false;

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            Logger.LogInformation("Starting temporary files cleanup");
            
            var result = await _tempCleaner.CleanupTempFilesAsync();
            
            if (result.Success)
            {
                Logger.LogInformation("Temporary files cleanup completed. Freed {Bytes} bytes", result.SpaceFreed);
                return CreateResult(true, "Temporary files cleaned successfully", spaceFreedBytes: result.SpaceFreed);
            }
            else
            {
                Logger.LogError("Temporary files cleanup failed: {Error}", result.ErrorMessage);
                return CreateResult(false, "Cleanup failed", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during temporary files cleanup");
            return CreateResult(false, "Cleanup failed", ex.Message);
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        return Task.FromResult(CreateResult(false, "This tweak is not reversible", "Temporary files cannot be restored after deletion"));
    }
}
