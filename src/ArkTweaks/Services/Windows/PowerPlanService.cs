using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.Services;

public class PowerPlanService
{
    private readonly ILogger<PowerPlanService> _logger;

    public PowerPlanService(ILogger<PowerPlanService> logger)
    {
        _logger = logger;
    }

    public class PowerPlan
    {
        public Guid Guid { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public List<PowerPlan> GetPowerPlans()
    {
        var plans = new List<PowerPlan>();

        try
        {
            // For MVP, return mock data
            // In production, this would use Windows API or PowerShell
            plans.Add(new PowerPlan
            {
                Guid = Guid.Parse("381b4222-f694-41f0-9685-ff5bb260df2e"),
                Name = "Balanced",
                IsActive = true
            });

            plans.Add(new PowerPlan
            {
                Guid = Guid.Parse("8c5e7fda-e8bf-45a6-a7cc-4b4a3d2e1c7c"),
                Name = "High Performance",
                IsActive = false
            });

            plans.Add(new PowerPlan
            {
                Guid = Guid.Parse("a1841308-3541-4fab-bc81-f71556f20b4a"),
                Name = "Power Saver",
                IsActive = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get power plans");
        }

        return plans;
    }

    public Task<PowerPlan?> GetCurrentPowerPlanAsync()
    {
        return Task.FromResult(GetPowerPlans().FirstOrDefault(p => p.IsActive));
    }

    public async Task<bool> SetPowerPlanAsync(string planName)
    {
        var plans = GetPowerPlans();
        var plan = plans.FirstOrDefault(p => p.Name.Equals(planName, StringComparison.OrdinalIgnoreCase));
        
        if (plan == null)
        {
            _logger.LogWarning("Power plan not found: {Name}", planName);
            return false;
        }

        return await SetPowerPlanByGuidAsync(plan.Guid.ToString());
    }

    public async Task<bool> SetPowerPlanByGuidAsync(string guidString)
    {
        try
        {
            var guid = Guid.Parse(guidString);
            SetPowerPlan(guid);
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set power plan by GUID");
            return false;
        }
    }

    public void SetPowerPlan(Guid planGuid)
    {
        try
        {
            // For MVP, just log
            // In production, this would use powercfg command or Windows API
            _logger.LogInformation("Setting power plan: {Guid}", planGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set power plan");
            throw;
        }
    }
}
