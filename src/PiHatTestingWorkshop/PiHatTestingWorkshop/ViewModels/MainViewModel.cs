using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PiHatTestingWorkshop.ViewModels;

// This is just for window selection gui. Most of the window size/position actual logic is in App.axaml.cs
public partial class MainViewModel(ILogger? loggerApplication = null) : ObservableObject
{
    [ObservableProperty] private string _pinToWorkText = "";
    
    [RelayCommand]
    private void whenViewLoaded(object? plot)
    {
        if (loggerApplication is null)
        {
            loggerApplication?.Error("In WhenViewLoaded() for MainViewModel, loggerApplication is null");
            
            throw new NullReferenceException();
        }
    }

    [RelayCommand]
    private void SetPinHigh()
    {
        Console.WriteLine($"Setting pin: {PinToWorkText} to HIGH");
    }

    [RelayCommand]
    private void SetPinLow()
    {
        Console.WriteLine($"Setting pin: {PinToWorkText} to LOW");
    }
}

