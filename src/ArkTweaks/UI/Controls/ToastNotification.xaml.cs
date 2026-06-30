using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ArkTweaks.UI.Controls;

public partial class ToastNotification : UserControl
{
    private readonly Storyboard _showStoryboard;
    private readonly Storyboard _hideStoryboard;

    public ToastNotification()
    {
        InitializeComponent();
        
        _showStoryboard = new Storyboard();
        _hideStoryboard = new Storyboard();
        
        var showOpacity = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        
        var showTranslate = new DoubleAnimation
        {
            From = -20,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        
        Storyboard.SetTarget(showOpacity, ToastBorder);
        Storyboard.SetTargetProperty(showOpacity, new PropertyPath("Opacity"));
        Storyboard.SetTarget(showTranslate, ToastBorder.RenderTransform);
        Storyboard.SetTargetProperty(showTranslate, new PropertyPath("Y"));
        
        _showStoryboard.Children.Add(showOpacity);
        _showStoryboard.Children.Add(showTranslate);
        
        var hideOpacity = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        
        var hideTranslate = new DoubleAnimation
        {
            From = 0,
            To = -20,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        
        Storyboard.SetTarget(hideOpacity, ToastBorder);
        Storyboard.SetTargetProperty(hideOpacity, new PropertyPath("Opacity"));
        Storyboard.SetTarget(hideTranslate, ToastBorder.RenderTransform);
        Storyboard.SetTargetProperty(hideTranslate, new PropertyPath("Y"));
        
        _hideStoryboard.Children.Add(hideOpacity);
        _hideStoryboard.Children.Add(hideTranslate);
        
        _hideStoryboard.Completed += (s, e) => Visibility = Visibility.Collapsed;
    }

    public string Title
    {
        get => TitleText.Text;
        set => TitleText.Text = value;
    }

    public string Message
    {
        get => MessageText.Text;
        set => MessageText.Text = value;
    }

    public string Icon
    {
        get => IconText.Text;
        set => IconText.Text = value;
    }

    public void Show()
    {
        Visibility = Visibility.Visible;
        _showStoryboard.Begin();
    }

    public void Hide()
    {
        _hideStoryboard.Begin();
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        Hide();
    }
}
