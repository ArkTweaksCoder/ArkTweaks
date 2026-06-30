using Microsoft.Extensions.Logging;

namespace ArkTweaks.UI.ViewModels;

public class AboutViewModel : BaseViewModel
{
    public string Version => "1.0.0";
    public string Description => "Ark Tweaks - A Windows optimization tool built for gamers and performance-focused users.";

    public AboutViewModel(ILogger<AboutViewModel> logger) 
        : base(logger)
    {
    }
}
