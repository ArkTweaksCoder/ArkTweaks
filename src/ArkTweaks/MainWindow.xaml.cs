using System;
using System.Windows;
using System.Windows.Controls;
using ArkTweaks.UI.Navigation;
using ArkTweaks.UI.ViewModels;
using ArkTweaks.UI.Views;

namespace ArkTweaks;

public partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;

    public MainWindow(NavigationService navigationService)
    {
        InitializeComponent();
        _navigationService = navigationService;
        
        // Subscribe to navigation changes
        _navigationService.PageChanged += OnPageChanged;
        
        // Load initial page
        LoadPage(_navigationService.CurrentPage);
    }

    private void OnNavButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string pageName)
        {
            if (Enum.TryParse<PageType>(pageName, out var pageType))
            {
                _navigationService.NavigateTo(pageType);
            }
        }
    }

    private void OnPageChanged(PageType pageType)
    {
        LoadPage(pageType);
    }

    private void LoadPage(PageType pageType)
    {
        UserControl? page = pageType switch
        {
            PageType.Dashboard => new DashboardPage(),
            PageType.Optimize => new OptimizePage(),
            PageType.Cleanup => new CleanupPage(),
            PageType.Startup => new StartupPage(),
            PageType.Gaming => new GamingPage(),
            PageType.Performance => new PerformancePage(),
            PageType.Restore => new RestorePage(),
            PageType.Settings => new SettingsPage(),
            PageType.About => new AboutPage(),
            _ => new DashboardPage()
        };

        ContentArea.Content = page;
    }

    protected override void OnClosed(EventArgs e)
    {
        _navigationService.PageChanged -= OnPageChanged;
        base.OnClosed(e);
    }
}