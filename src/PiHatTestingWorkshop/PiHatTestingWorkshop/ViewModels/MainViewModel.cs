using System.Device.Gpio;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PiHatTestingWorkshop.ViewModels;

// This is just for window selection gui. Most of the window size/position actual logic is in App.axaml.cs
public partial class MainViewModel(ILogger? loggerApplication = null) : ObservableObject
{
    [ObservableProperty] private string _pinToWorkText = "";
    
    private GpioController? _gpio;

    [RelayCommand]
    private void whenViewLoaded(object? plot)
    {
        if (loggerApplication is null)
        {
            loggerApplication?.Error("In WhenViewLoaded() for MainViewModel, loggerApplication is null");
            
            throw new NullReferenceException();
        }

        _gpio = new GpioController();
    }

    [RelayCommand]
    private void SetPinHigh()
    {
        var convertedPin = convertPinString(PinToWorkText);

        if (convertedPin == 0) return;
        
        // Otherwise, converted textbox string to a valid int
        Console.WriteLine($"Setting pin: {PinToWorkText} to HIGH");
            
        if (_gpio is null) throw new NullReferenceException("_gpio is null, cannot proceed");

        _gpio.Write(convertedPin, true);
    }
    
    [RelayCommand]
    private void SetPinLow()
    {
        var convertedPin = convertPinString(PinToWorkText);

        if (convertedPin == 0) return;
        
        // Otherwise, converted textbox string to a valid int
        Console.WriteLine($"Setting pin: {PinToWorkText} to LOW");
            
        if (_gpio is null) throw new NullReferenceException("_gpio is null, cannot proceed");

        _gpio.Write(convertedPin, false);
    }   
    
    [RelayCommand]
    private void ReadPinValuesRepeatedly()
    {
        var countdown = 500;
        
        var convertedPin = convertPinString(PinToWorkText);

        if (convertedPin == 0) return;
        
        // Otherwise, converted textbox string to a valid int
        if (_gpio is null) throw new NullReferenceException("_gpio is null, cannot proceed");
            
        _gpio.OpenPin(convertedPin, PinMode.Input);

        try
        {
            while (countdown-- > 0)
            {
                var pinValue = _gpio.Read(convertedPin);
                
                Console.WriteLine($"[Pin {convertedPin}] is currently {pinValue}");
            }
        }
        finally
        {
            _gpio.ClosePin(convertedPin);
        }
        
        Console.WriteLine($"Finished looping pin read");
    }
    
    private int convertPinString(string pinToWorkText)
    {
        try
        {
            var pinInt = int.Parse(pinToWorkText);

            if (pinInt < 1) throw new ArgumentOutOfRangeException(nameof(pinInt));

            if (pinInt > 40) throw new ArgumentOutOfRangeException(nameof(pinInt));

            return pinInt;
        }
        catch (Exception)
        {
            PinToWorkText = "INVALID PIN";
            
            return 0;
        }
    }
}

