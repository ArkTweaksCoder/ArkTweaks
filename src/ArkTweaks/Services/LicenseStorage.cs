using System;
using System.IO;
using System.Text.Json;
using ArkTweaks.Models;

namespace ArkTweaks.Services;

public class LicenseStorage
{
    private readonly string _licenseFilePath;

    public LicenseStorage()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "ArkTweaks");
        
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        _licenseFilePath = Path.Combine(appFolder, "license.json");
    }

    public LicenseInfo LoadLicense()
    {
        if (!File.Exists(_licenseFilePath))
        {
            return new LicenseInfo { Tier = LicenseTier.Free, IsValid = true };
        }

        try
        {
            var json = File.ReadAllText(_licenseFilePath);
            var license = JsonSerializer.Deserialize<LicenseInfo>(json);
            
            if (license == null)
            {
                return new LicenseInfo { Tier = LicenseTier.Free, IsValid = true };
            }

            // Check expiration
            if (license.ExpirationDate.HasValue && license.ExpirationDate < DateTime.UtcNow)
            {
                return new LicenseInfo { Tier = LicenseTier.Free, IsValid = true, ErrorMessage = "License expired" };
            }

            return license;
        }
        catch
        {
            return new LicenseInfo { Tier = LicenseTier.Free, IsValid = true };
        }
    }

    public void SaveLicense(LicenseInfo license)
    {
        var json = JsonSerializer.Serialize(license, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_licenseFilePath, json);
    }

    public void ClearLicense()
    {
        if (File.Exists(_licenseFilePath))
        {
            File.Delete(_licenseFilePath);
        }
    }
}
