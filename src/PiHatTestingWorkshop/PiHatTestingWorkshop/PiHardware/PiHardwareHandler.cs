using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace PiHatTestingWorkshop.PiHardware;

public class PiHardwareHandler
{
    private GpioController? _gpio;

    public void SetPiGpioPin(int pinToWork, bool newPinState)
    {
        _gpio ??= new GpioController();
        
        if (pinToWork is < 1 or > 40)
        {
            Console.WriteLine($"Pin passed to SetPiGpioPin was: {pinToWork}, which is invalid. Stopping now");
            return;
        }
        
        // Otherwise, converted textbox string to a valid int
        Console.WriteLine($"Setting pin: {pinToWork} to {newPinState}");
            
        if (_gpio is null) throw new NullReferenceException("_gpio is null, cannot proceed");

        _gpio.Write(pinToWork, newPinState);
    }
    
    public void ReadPiGpioPinRepeatedly(int pinToWork)
    {
        _gpio ??= new GpioController();
        
        var countdown = 500;
        
        if (pinToWork is < 1 or > 40)
        {
            Console.WriteLine($"Pin passed to ReadPinValuesRepeatedly was: {pinToWork}, which is invalid. Stopping now");
            return;
        }
        
        // Otherwise, converted textbox string to a valid int
        if (_gpio is null) throw new NullReferenceException("_gpio is null, cannot proceed");
            
        _gpio.OpenPin(pinToWork, PinMode.Input);

        try
        {
            while (countdown-- > 0)
            {
                var pinValue = _gpio.Read(pinToWork);
                
                Console.WriteLine($"[Pin {pinToWork}] is currently {pinValue}");
            }
        }
        finally
        {
            _gpio.ClosePin(pinToWork);
        }
        
        Console.WriteLine($"Finished looping pin read");
    }
    
    public void ReadWaveshareAdDaHatRepeatedly()
    {
        _gpio ??= new GpioController();

        var adcHatHandler = new WaveshareAdcHatHandler(_gpio);

        adcHatHandler.InitAds1256();
        
        var waveshareAdcDeviceId = adcHatHandler.GetAds1256DeviceId();
        
        Console.WriteLine($"ADS1256 Device ID from board is: {waveshareAdcDeviceId}");
        
        var countdown = 5000;
        
        while (countdown-- > 0)
        {
            var pin00AnalogValue = adcHatHandler.ReadAdcSingleChannel(0);
            var pin01AnalogValue = adcHatHandler.ReadAdcSingleChannel(1);
            var pin02AnalogValue = adcHatHandler.ReadAdcSingleChannel(2);
            var pin03AnalogValue = adcHatHandler.ReadAdcSingleChannel(3);
            
            Console.WriteLine($"DeviceID: {waveshareAdcDeviceId} | [ADC 0] is currently {pin00AnalogValue} | " + 
                              $"[ADC 1] is currently {pin01AnalogValue} | " + 
                              $"[ADC 2] is currently {pin02AnalogValue} | " +
                              $"[ADC 3] is currently {pin03AnalogValue}");
        }
        
        Console.WriteLine($"Finished looping Waveshare AD/DA HAT read");
    }
    
    public static int ConvertPinString(string pinToWorkText)
    {
        var pinInt = int.Parse(pinToWorkText);

        if (pinInt < 1) throw new ArgumentOutOfRangeException(nameof(pinInt), "While trying to convert pin string, pin seems invalid");

        if (pinInt > 40) throw new ArgumentOutOfRangeException(nameof(pinInt), "While trying to convert pin string, pin seems invalid");

        return pinInt;
    }
}