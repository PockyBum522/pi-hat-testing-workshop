using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace PiHatTestingWorkshop.PiHardware;

public class WaveshareAdcHatHandler(GpioController gpio)
{
    private const int _dataReadyPin = 17;        // 17 by other numbering
    private const int _chipSelectPin = 22;       // 22 by other numbering
    private const int _resetPin = 18;            // 18 by other numbering
    
    private SpiDevice? _ads1256;

    public void ReadWaveshareAdDaHatRepeatedly()
    {
        InitAds1256();
        
        var waveshareAdcDeviceId = GetAds1256DeviceId();
        
        Console.WriteLine($"ADS1256 Device ID from board is: {waveshareAdcDeviceId}");
        
        var countdown = 500;
        
        while (countdown-- > 0)
        {
            var pin00AnalogValue = ReadAdcSingleChannel(0);
            var pin01AnalogValue = ReadAdcSingleChannel(1);
            var pin02AnalogValue = ReadAdcSingleChannel(2);
            var pin03AnalogValue = ReadAdcSingleChannel(3);
            
            Console.WriteLine($"DeviceID: {waveshareAdcDeviceId} | [ADC 0] is currently {pin00AnalogValue} | " + 
                              $"[ADC 1] is currently {pin01AnalogValue} | " + 
                              $"[ADC 2] is currently {pin02AnalogValue} | " +
                              $"[ADC 3] is currently {pin03AnalogValue}");
        }
    }

    private void InitAds1256()
    { 
        if (gpio is null) throw new NullReferenceException("gpio is null");
        
        gpio.OpenPin(_resetPin, PinMode.Output);

        gpio.OpenPin(_chipSelectPin, PinMode.Output);
        
        gpio.OpenPin(_dataReadyPin, PinMode.InputPullUp);
        
        var settings = new SpiConnectionSettings(0, 0)
        {
            ClockFrequency = 20000,             // SPI.max_speed_hz = 20000 
            Mode = SpiMode.Mode1,               // SPI.mode = 0b01 (BUT TEST MODE 0 IF THIS DOESN'T WORK)  
        };
        
        _ads1256 = SpiDevice.Create(settings);

        resetAds1256();
    }

    private void resetAds1256()
    {
        if (gpio is null) throw new NullReferenceException("gpio is null");
        
        gpio.Write(_resetPin, PinValue.High);
        Thread.Sleep(200);
        gpio.Write(_resetPin, PinValue.Low);
        Thread.Sleep(200);
        gpio.Write(_resetPin, PinValue.High);
    }

    private int GetAds1256DeviceId()
    {
        waitForDataReadyAds1256();
        
        var returnedData = readDataAds1256(0);

        return returnedData >> 4;
    }

    private int readDataAds1256(int reg)
    {
        if (gpio is null ||
            _ads1256 is null) throw new NullReferenceException();
        
        gpio.Write(_chipSelectPin, PinValue.Low);
        
        Span<byte> writeBuffer = [0x10, 0x00];      // RREG command, STATUS register, 1 byte to read
        _ads1256.Write(writeBuffer);
        
        Span<byte> readBuffer = stackalloc byte[1];
        _ads1256.Read(readBuffer);
        
        gpio.Write(_chipSelectPin, PinValue.High);

        return readBuffer[0];
    }

    private void waitForDataReadyAds1256()
    {
        if (gpio is null) throw new NullReferenceException("gpio is null");
        
        for (var i = 0; i < 400000; i++)
        {
            if (gpio.Read(_dataReadyPin) == 0)
            {
                break;
            }

            if (i >= 399999)
            {
                Console.WriteLine("Timed out waiting for data ready pin");
            }
        }
    }

    private double ReadAdcSingleChannel(byte channel)
    {
        setChannelAds1256(channel);

        var cmdSync = (byte)0xFC;
        var cmdWakeup = (byte)0x00;
        
        writeCommandAds1256(cmdSync);

        writeCommandAds1256(cmdWakeup);
        
        var channelValue = readAdcDataAds1256();
        
        return channelValue;
    }

    private double readAdcDataAds1256()
    {
        
        if (gpio is null ||
            _ads1256 is null) throw new NullReferenceException();
        
        waitForDataReadyAds1256();
        
        gpio.Write(_chipSelectPin, PinValue.Low);

        var rData = (byte)0x01;
        
        _ads1256.WriteByte(rData);
        
        Span<byte> readBuffer = stackalloc byte[3];
        _ads1256.Read(readBuffer);

        gpio.Write(_chipSelectPin, PinValue.High);

        var finalValue = (readBuffer[0] << 16) & 0xff0000;
        finalValue |= (readBuffer[1] << 8) & 0xff00;
        finalValue |= readBuffer[2] & 0xFF;

        return finalValue;
    }

    private void writeCommandAds1256(byte reg)
    {
        
        if (gpio is null ||
            _ads1256 is null) throw new NullReferenceException();
        
        gpio.Write(_chipSelectPin, PinValue.Low);
        
        _ads1256.WriteByte(reg);
        
        gpio.Write(_chipSelectPin, PinValue.High);
    }

    private void setChannelAds1256(int channel)
    {
        if (channel > 7)
            throw new ArgumentOutOfRangeException(nameof(channel), "Channel out of range");

        var regMux = 1;
        
        writeRegAds1256((byte)regMux, channel);
    }

    private void writeRegAds1256(byte reg, int data)
    {        
        if (gpio is null ||
            _ads1256 is null) throw new NullReferenceException();
        
        gpio.Write(_chipSelectPin, PinValue.Low);
        
        var muxValue = (byte)((data << 4) | 0x08);
        var wReg = (byte)0x50;
        _ads1256.Write(new byte[] { (byte)(wReg | reg), 0x00, muxValue });
        
        gpio.Write(_chipSelectPin, PinValue.Low);
    }
}