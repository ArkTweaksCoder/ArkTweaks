using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArkTweaks.Core.Safety;
using ArkTweaks.Core.Tweaks;
using ArkTweaks.Models;
using ArkTweaks.Services;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Core.Engine;

public class OptimizationEngine
{
    private readonly ILogger<OptimizationEngine> _logger;
    private readonly LicenseService _licenseService;
    private readonly SafetyValidator _safetyValidator;
    private readonly ActionLogger _actionLogger;
    private readonly List<ITweak> _tweaks;

    public OptimizationEngine(
        ILogger<OptimizationEngine> logger,
        LicenseService licenseService,
        SafetyValidator safetyValidator,
        ActionLogger actionLogger)
    {
        _logger = logger;
        _licenseService = licenseService;
        _safetyValidator = safetyValidator;
        _actionLogger = actionLogger;
        _tweaks = new List<ITweak>();
    }

    public void RegisterTweak(ITweak tweak)
    {
        _tweaks.Add(tweak);
        _logger.LogInformation("Registered tweak: {TweakName} ({TweakId})", tweak.Name, tweak.Id);
    }

    public IReadOnlyList<ITweak> GetAllTweaks()
    {
        return _tweaks.AsReadOnly();
    }

    public IReadOnlyList<ITweak> GetAvailableTweaks()
    {
        var currentLicense = _licenseService.GetCurrentLicenseTier();
        
        return _tweaks
            .Where(t => t.RequiredLicenseTier <= currentLicense)
            .Where(t => t.IsAvailable())
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyList<ITweak> GetTweaksByCategory(TweakCategory category)
    {
        return GetAvailableTweaks()
            .Where(t => t.Category == category)
            .ToList()
            .AsReadOnly();
    }

    public ITweak? GetTweakById(string id)
    {
        return _tweaks.FirstOrDefault(t => t.Id == id);
    }

    public async Task<TweakResult> ExecuteTweakAsync(string tweakId)
    {
        var tweak = GetTweakById(tweakId);
        if (tweak == null)
        {
            return new TweakResult
            {
                TweakId = tweakId,
                Success = false,
                Message = "Tweak not found",
                ErrorMessage = $"No tweak found with ID: {tweakId}"
            };
        }

        // License check
        var currentLicense = _licenseService.GetCurrentLicenseTier();
        if (tweak.RequiredLicenseTier > currentLicense)
        {
            return new TweakResult
            {
                TweakId = tweakId,
                Success = false,
                Message = "License tier insufficient",
                ErrorMessage = $"This tweak requires {tweak.RequiredLicenseTier} license or higher"
            };
        }

        // Safety validation
        var safetyResult = _safetyValidator.ValidateTweak(tweak);
        if (!safetyResult.IsValid)
        {
            return new TweakResult
            {
                TweakId = tweakId,
                Success = false,
                Message = "Safety validation failed",
                ErrorMessage = safetyResult.Reason
            };
        }

        // Execute
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("Executing tweak: {TweakName}", tweak.Name);
            var result = await tweak.ApplyAsync();
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
            
            // Log action
            _actionLogger.LogAction(tweakId, tweak.Name, result.Success, result.ErrorMessage);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error executing tweak: {TweakName}", tweak.Name);
            
            var result = new TweakResult
            {
                TweakId = tweakId,
                Success = false,
                Message = "Execution failed",
                ErrorMessage = ex.Message,
                ExecutionTime = stopwatch.Elapsed
            };
            
            _actionLogger.LogAction(tweakId, tweak.Name, false, ex.Message);
            return result;
        }
    }

    public async Task<TweakResult> RevertTweakAsync(string tweakId)
    {
        var tweak = GetTweakById(tweakId);
        if (tweak == null)
        {
            return new TweakResult
            {
                TweakId = tweakId,
                Success = false,
                Message = "Tweak not found",
                ErrorMessage = $"No tweak found with ID: {tweakId}"
            };
        }

        if (!tweak.IsReversible)
        {
            return new TweakResult
            {
                TweakId = tweakId,
                Success = false,
                Message = "Tweak is not reversible",
                ErrorMessage = "This tweak cannot be reverted"
            };
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("Reverting tweak: {TweakName}", tweak.Name);
            var result = await tweak.RevertAsync();
            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
            
            _actionLogger.LogAction(tweakId, $"{tweak.Name} (Revert)", result.Success, result.ErrorMessage);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error reverting tweak: {TweakName}", tweak.Name);
            
            var result = new TweakResult
            {
                TweakId = tweakId,
                Success = false,
                Message = "Revert failed",
                ErrorMessage = ex.Message,
                ExecutionTime = stopwatch.Elapsed
            };
            
            _actionLogger.LogAction(tweakId, $"{tweak.Name} (Revert)", false, ex.Message);
            return result;
        }
    }

    public async Task<List<TweakResult>> ExecuteMultipleTweaksAsync(List<string> tweakIds)
    {
        var results = new List<TweakResult>();
        
        foreach (var tweakId in tweakIds)
        {
            var result = await ExecuteTweakAsync(tweakId);
            results.Add(result);
            
            // Stop if a critical tweak fails
            if (!result.Success && GetTweakById(tweakId)?.RiskLevel >= RiskLevel.Medium)
            {
                _logger.LogWarning("Stopping execution due to critical tweak failure: {TweakId}", tweakId);
                break;
            }
        }
        
        return results;
    }
}
