using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PiHatTestingWorkshop.PiHardware;

namespace PiHatTestingWorkshop.ViewModels;

// This is just for window selection gui. Most of the window size/position actual logic is in App.axaml.cs
public partial class MainViewModel(ILogger? loggerApplication = null, PiHardwareHandler? piHardware = null) : ObservableObject
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
        var convertedPin = PiHardwareHandler.ConvertPinString(PinToWorkText);
        
        // Otherwise, converted textbox string to a valid int
        Console.WriteLine($"Setting pin: {PinToWorkText} to HIGH");
        
        if (piHardware is null) throw new NullReferenceException();
        
        piHardware.SetPiGpioPin(convertedPin, true);
    }
    
    [RelayCommand]
    private void SetPinLow()
    {
        var convertedPin = PiHardwareHandler.ConvertPinString(PinToWorkText);
        
        // Otherwise, converted textbox string to a valid int
        Console.WriteLine($"Setting pin: {PinToWorkText} to LOW");
        
        if (piHardware is null) throw new NullReferenceException();
        
        piHardware.SetPiGpioPin(convertedPin, false);
    }   
    
    [RelayCommand]
    private void ReadPinValuesRepeatedly()
    {
        var convertedPin = PiHardwareHandler.ConvertPinString(PinToWorkText);

        if (convertedPin == 0)
        {
            Console.WriteLine("Converted pin seems invalid in ReadPinValuesRepeatedly()");
            return;
        }
        
        // Otherwise, converted textbox string to a valid int
        Console.WriteLine($"Setting pin: {PinToWorkText} to HIGH");
        
        if (piHardware is null) throw new NullReferenceException();
        
        piHardware.ReadPiGpioPinRepeatedly(convertedPin);
        
        Console.WriteLine($"Finished looping pin read");
    }
    
    [RelayCommand]
    private void ReadWaveshareAdDaHatRepeatedly()
    {
        // Otherwise, converted textbox string to a valid int
        Console.WriteLine("Starting looping Waveshare AD/DA HAT read");
        
        if (piHardware is null) throw new NullReferenceException();
        
        piHardware.ReadWaveshareAdDaHatRepeatedly();
        
        Console.WriteLine("Finished looping Waveshare AD/DA HAT read");
    }
}

