using System;

namespace ArkTweaks.Models;

public class LicenseInfo
{
    public LicenseTier Tier { get; set; } = LicenseTier.Free;
    public string LicenseKey { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
