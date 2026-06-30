using ArkTweaks.Core.Safety;
using ArkTweaks.Models;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Core.Tweaks;

public abstract class BaseTweak : ITweak
{
    protected readonly ILogger<BaseTweak> Logger;

    protected BaseTweak(ILogger<BaseTweak> logger)
    {
        Logger = logger;
    }

    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract TweakCategory Category { get; }
    public abstract LicenseTier RequiredLicenseTier { get; }
    public abstract RiskLevel RiskLevel { get; }
    public abstract bool IsReversible { get; }

    public virtual bool IsAvailable()
    {
        return true;
    }

    public abstract Task<TweakResult> ApplyAsync();
    public abstract Task<TweakResult> RevertAsync();

    protected TweakResult CreateResult(bool success, string message, string? errorMessage = null, long? spaceFreedBytes = null)
    {
        return new TweakResult
        {
            TweakId = Id,
            Success = success,
            Message = message,
            ErrorMessage = errorMessage,
            Timestamp = DateTime.UtcNow,
            SpaceFreedBytes = spaceFreedBytes
        };
    }
}
