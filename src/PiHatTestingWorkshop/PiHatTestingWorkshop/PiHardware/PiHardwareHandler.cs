using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace PiHatTestingWorkshop.PiHardware;

public class PiHardwareHandler
{
    private GpioController? _gpio;
    
    public void SetPiGpioPin(int pinToWork, bool newPinState)
    {
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
        
        // GPIO pins for various control lines
        const int chipSelectPin = 8;     // Chip Select (CE0)
        const int resetPin = 22;   // Reset

        _gpio.OpenPin(resetPin, PinMode.Output);
        _gpio.OpenPin(chipSelectPin, PinMode.Output);
        
        // Configure the ADS1256 control pins
        _gpio.Write(resetPin, PinValue.High);
        _gpio.Write(chipSelectPin, PinValue.High);
        
        // Configure SPI settings
        var settings = new SpiConnectionSettings(0, chipSelectPin)
        {
            ClockFrequency = 1000000, // 1 MHz
            Mode = SpiMode.Mode1,     // CPOL = 0, CPHA = 1
            DataBitLength = 8,
        };

        // Create SPI device
        var ads1256 = SpiDevice.Create(settings);

        // Send a command and read device ID
        readSpiDeviceId(ads1256);
        
        
        Console.WriteLine("Starting ADS1256 reads:");
        
        var countdown = 500;
        
        while (countdown-- > 0)
        {
            var pin00AnalogValue = readAdcSingleChannel(0, ads1256);
            var pin01AnalogValue = readAdcSingleChannel(1, ads1256);
            var pin02AnalogValue = readAdcSingleChannel(2, ads1256);
            var pin03AnalogValue = readAdcSingleChannel(3, ads1256);
            
            Console.WriteLine($"[ADC 0] is currently {pin00AnalogValue} | " + 
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
    
    private static void readSpiDeviceId(SpiDevice ads1256)
    {
        // Command to read the device ID from the STATUS register
        Span<byte> writeBuffer = [0x10, 0x00];      // RREG command, STATUS register, 1 byte to read
        Span<byte> readBuffer = stackalloc byte[1];

        // Send command and receive response
        ads1256.TransferFullDuplex(writeBuffer, readBuffer);

        var deviceId = readBuffer[0];
        Console.WriteLine($"ADS1256 Device ID: {deviceId:X2}");
    }
    
    private int readAdcSingleChannel(byte channel, SpiDevice ads1256)
    {
        _gpio ??= new GpioController();
        
        const int dataReadyPin = 17;
        
        // 1. Send the RDATAC command to exit continuous mode (if active)
        ads1256.Write(new byte[] { 0x01 });

        // 2. Write the MUX register to select the desired channel
        byte muxRegCommand = (byte)(0x50 | 0x01); // WREG command for MUX (0x01)
        byte muxValue = (byte)((channel << 4) | 0x08); // INP = channel, INN = AINCOM
        ads1256.Write(new byte[] { muxRegCommand, 0x00, muxValue });

        // 3. Send the SYNC command
        ads1256.Write(new byte[] { 0x04 });

        // 4. Send the WAKEUP command
        ads1256.Write(new byte[] { 0x00 });
        Thread.Sleep(10); // Settle time

        // 5. Wait for DRDY pin to go low (indicating data is ready)
        while (_gpio.Read(dataReadyPin) == PinValue.High) { }

        // 6. Send the RDATA command
        Span<byte> writeBuffer = stackalloc byte[] { 0x10 };
        Span<byte> readBuffer = stackalloc byte[3]; // Read 3 bytes for 24-bit data
        ads1256.TransferFullDuplex(writeBuffer, readBuffer);

        // 7. Combine the 3 bytes into a single integer
        int result = (readBuffer[0] << 16) | (readBuffer[1] << 8) | readBuffer[2];
    
        // Handle the 24-bit signed representation
        if ((result & 0x800000) != 0)
        {
            result |= ~0xFFFFFF;
        }
        return result;
    }
    
}