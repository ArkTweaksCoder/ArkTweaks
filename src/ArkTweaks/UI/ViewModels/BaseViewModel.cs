using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace ArkTweaks.UI.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    protected readonly ILogger Logger;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected BaseViewModel(ILogger logger)
    {
        Logger = logger;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
