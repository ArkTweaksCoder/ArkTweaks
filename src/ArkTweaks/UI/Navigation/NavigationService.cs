using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.UI.Navigation;

public class NavigationService : INotifyPropertyChanged
{
    private readonly ILogger<NavigationService> _logger;
    private PageType _currentPage;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<PageType>? PageChanged;

    public PageType CurrentPage
    {
        get => _currentPage;
        private set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                OnPropertyChanged();
                PageChanged?.Invoke(value);
                _logger.LogDebug("Navigated to page: {Page}", value);
            }
        }
    }

    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger;
        _currentPage = PageType.Dashboard;
    }

    public void NavigateTo(PageType pageType)
    {
        _logger.LogInformation("Navigation requested: {Page}", pageType);
        CurrentPage = pageType;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
