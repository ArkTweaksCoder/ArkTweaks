using System;
using System.IO;
using System.Threading.Tasks;
using ArkTweaks.Core.Safety;
using ArkTweaks.Core.Tweaks;
using ArkTweaks.Models;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Tweaks.Storage;

public class RecycleBinCleanupTweak : BaseTweak
{
    public RecycleBinCleanupTweak(ILogger<RecycleBinCleanupTweak> logger) : base(logger)
    {
    }

    public override string Id => "cleanup_recycle_bin";
    public override string Name => "Recycle Bin Cleanup";
    public override string Description => "Empties the Windows recycle bin to free disk space.";
    public override TweakCategory Category => TweakCategory.Cleanup;
    public override LicenseTier RequiredLicenseTier => LicenseTier.Standard;
    public override RiskLevel RiskLevel => RiskLevel.Low;
    public override bool IsReversible => false;

    public override async Task<TweakResult> ApplyAsync()
    {
        try
        {
            Logger.LogInformation("Starting recycle bin cleanup");
            
            var spaceFreed = await EmptyRecycleBinAsync();
            
            Logger.LogInformation("Recycle bin cleanup completed. Freed {Bytes} bytes", spaceFreed);
            return CreateResult(true, "Recycle bin emptied successfully", spaceFreedBytes: spaceFreed);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during recycle bin cleanup");
            return CreateResult(false, "Cleanup failed", ex.Message);
        }
    }

    public override Task<TweakResult> RevertAsync()
    {
        return Task.FromResult(CreateResult(false, "This tweak is not reversible", "Recycle bin cannot be restored after emptying"));
    }

    private async Task<long> EmptyRecycleBinAsync()
    {
        long spaceFreed = 0;
        
        // Get recycle bin size before emptying
        var recycleBinPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "$Recycle.Bin");
        if (Directory.Exists(recycleBinPath))
        {
            spaceFreed = CalculateDirectorySize(recycleBinPath);
        }

        // Use Shell to empty recycle bin
        await Task.Run(() =>
        {
            var shellType = Type.GetTypeFromProgID("Shell.Application");
            if (shellType != null)
            {
                var shell = Activator.CreateInstance(shellType);
                if (shell != null)
                {
                    dynamic shellObj = shell;
                    shellObj.Namespace(10).Self.InvokeVerb("Empty Recycle Bin");
                }
            }
        });

        return spaceFreed;
    }

    private long CalculateDirectorySize(string path)
    {
        long size = 0;
        
        try
        {
            var dirInfo = new DirectoryInfo(path);
            
            foreach (var file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                try
                {
                    size += file.Length;
                }
                catch
                {
                    // Skip files that can't be accessed
                }
            }
        }
        catch
        {
            // Return 0 if we can't calculate
        }
        
        return size;
    }
}
