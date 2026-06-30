using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;
using ArkTweaks.Core.Engine;

namespace ArkTweaks.UI.ViewModels;

public class PerformanceViewModel : BaseViewModel
{
    private readonly LicenseService _licenseService;
    private readonly PowerPlanService _powerPlanService;
    private readonly OptimizationEngine _optimizationEngine;
    private bool _isLoading;
    private string _currentPlan = "Balanced";

    public ObservableCollection<PowerPlanViewModel> PowerPlans { get; } = new();

    public string CurrentPlan
    {
        get => _currentPlan;
        set => SetProperty(ref _currentPlan, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool CanSwitch => _licenseService.IsFeatureEnabled(LicenseFeature.PowerPlanSwitch);

    public PerformanceViewModel(
        ILogger<PerformanceViewModel> logger,
        LicenseService licenseService,
        PowerPlanService powerPlanService,
        OptimizationEngine optimizationEngine) 
        : base(logger)
    {
        _licenseService = licenseService;
        _powerPlanService = powerPlanService;
        _optimizationEngine = optimizationEngine;
        LoadPowerPlans();
    }

    public void LoadPowerPlans()
    {
        IsLoading = true;
        try
        {
            var plans = _powerPlanService.GetPowerPlans();
            PowerPlans.Clear();
            
            foreach (var plan in plans)
            {
                PowerPlans.Add(new PowerPlanViewModel
                {
                    Name = plan.Name,
                    Guid = plan.Guid.ToString(),
                    IsActive = plan.IsActive
                });

                if (plan.IsActive)
                {
                    CurrentPlan = plan.Name;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load power plans");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SwitchPowerPlanAsync(PowerPlanViewModel plan)
    {
        if (!CanSwitch) return;

        IsLoading = true;
        try
        {
            var result = await _optimizationEngine.ExecuteTweakAsync("power_high_performance");
            
            if (result.Success)
            {
                CurrentPlan = plan.Name;
                LoadPowerPlans(); // Refresh to update active status
            }
            else
            {
                Logger.LogError("Failed to switch power plan: {Error}", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to switch power plan");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Refresh()
    {
        LoadPowerPlans();
    }
}

public class PowerPlanViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Guid { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
