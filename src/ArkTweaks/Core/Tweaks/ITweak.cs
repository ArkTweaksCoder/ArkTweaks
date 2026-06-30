using ArkTweaks.Core.Safety;
using ArkTweaks.Models;

namespace ArkTweaks.Core.Tweaks;

public interface ITweak
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    TweakCategory Category { get; }
    LicenseTier RequiredLicenseTier { get; }
    RiskLevel RiskLevel { get; }
    bool IsReversible { get; }
    
    bool IsAvailable();
    Task<TweakResult> ApplyAsync();
    Task<TweakResult> RevertAsync();
}
