using ArkTweaks.Models;

namespace ArkTweaks.Services;

public class LicenseValidator
{
    private const string LicensePrefix = "ARK";

    public (bool IsValid, LicenseTier Tier, string ErrorMessage) ValidateLicenseKey(string licenseKey)
    {
        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            return (false, LicenseTier.Free, "License key is empty");
        }

        var parts = licenseKey.Split('-');
        
        if (parts.Length != 4)
        {
            return (false, LicenseTier.Free, "Invalid license key format");
        }

        if (parts[0] != LicensePrefix)
        {
            return (false, LicenseTier.Free, "Invalid license key prefix");
        }

        // Validate tier suffix
        var tierSuffix = parts[3].ToUpper();
        LicenseTier tier = tierSuffix switch
        {
            "STD" => LicenseTier.Standard,
            "PRO" => LicenseTier.Pro,
            "ULT" => LicenseTier.Ultimate,
            _ => LicenseTier.Free
        };

        if (tier == LicenseTier.Free)
        {
            return (false, LicenseTier.Free, "Invalid license tier");
        }

        // In production, this would validate against a server
        // For MVP, we accept any properly formatted key
        return (true, tier, string.Empty);
    }
}
