using Microsoft.Extensions.Logging;
using ArkTweaks.Core.Tweaks;

namespace ArkTweaks.Core.Safety;

public class SafetyValidator
{
    private readonly ILogger<SafetyValidator> _logger;

    public SafetyValidator(ILogger<SafetyValidator> logger)
    {
        _logger = logger;
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Reason { get; set; } = string.Empty;
        public RiskLevel RiskLevel { get; set; }
    }

    public ValidationResult ValidateAction(string actionName, RiskLevel riskLevel, string description)
    {
        // Block high-risk actions
        if (riskLevel == RiskLevel.High)
        {
            _logger.LogWarning("Blocked high-risk action: {Action}", actionName);
            return new ValidationResult
            {
                IsValid = false,
                Reason = $"Action '{actionName}' is blocked due to high risk level.",
                RiskLevel = riskLevel
            };
        }

        // Warn on medium-risk actions
        if (riskLevel == RiskLevel.Medium)
        {
            _logger.LogInformation("Medium-risk action validated: {Action}", actionName);
            return new ValidationResult
            {
                IsValid = true,
                Reason = $"Action '{actionName}' requires caution. {description}",
                RiskLevel = riskLevel
            };
        }

        // Allow low-risk and none-risk actions
        _logger.LogDebug("Low-risk action validated: {Action}", actionName);
        return new ValidationResult
        {
            IsValid = true,
            Reason = string.Empty,
            RiskLevel = riskLevel
        };
    }

    public bool CanExecuteAction(string actionName, RiskLevel riskLevel)
    {
        var result = ValidateAction(actionName, riskLevel, string.Empty);
        return result.IsValid;
    }

    public ValidationResult ValidateTweak(ITweak tweak)
    {
        return ValidateAction(tweak.Name, tweak.RiskLevel, tweak.Description);
    }
}
