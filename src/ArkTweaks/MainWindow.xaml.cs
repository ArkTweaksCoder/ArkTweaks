using System;
using System.Windows;
using System.Windows.Controls;
using ArkTweaks.UI.Navigation;
using ArkTweaks.UI.ViewModels;
using ArkTweaks.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ArkTweaks;

public partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(NavigationService navigationService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;
        
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

        // Set DataContext with ViewModel
        if (page != null)
        {
            object? viewModel = pageType switch
            {
                PageType.Dashboard => _serviceProvider.GetService<DashboardViewModel>(),
                PageType.Optimize => _serviceProvider.GetService<OptimizeViewModel>(),
                PageType.Cleanup => _serviceProvider.GetService<CleanupViewModel>(),
                PageType.Startup => _serviceProvider.GetService<StartupViewModel>(),
                PageType.Gaming => _serviceProvider.GetService<GamingViewModel>(),
                PageType.Performance => _serviceProvider.GetService<PerformanceViewModel>(),
                PageType.Restore => _serviceProvider.GetService<RestoreViewModel>(),
                PageType.Settings => _serviceProvider.GetService<SettingsViewModel>(),
                PageType.About => _serviceProvider.GetService<AboutViewModel>(),
                _ => _serviceProvider.GetService<DashboardViewModel>()
            };
            
            page.DataContext = viewModel;
        }

        ContentArea.Content = page;
    }

    protected override void OnClosed(EventArgs e)
    {
        _navigationService.PageChanged -= OnPageChanged;
        base.OnClosed(e);
    }
}