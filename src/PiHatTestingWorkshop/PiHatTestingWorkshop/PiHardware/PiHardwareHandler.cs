using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace PiHatTestingWorkshop.PiHardware;

public class PiHardwareHandler
{
    private const int _dataReadyPin = 17;        // 17 by other numbering
    private const int _chipSelectPin = 22;       // 22 by other numbering
    private const int _resetPin = 18;            // 18 by other numbering
    
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

        initAds1256();
        
        
        
        // _gpio.OpenPin(_resetPin, PinMode.Output);
        // _gpio.OpenPin(_chipSelectPin, PinMode.Output);
        //
        // // Configure the ADS1256 control pins
        // _gpio.Write(_resetPin, PinValue.High);
        // _gpio.Write(_chipSelectPin, PinValue.High);
        //
        // // Configure SPI settings
        // var settings = new SpiConnectionSettings(0, 0)
        // {
        //     ClockFrequency = 20000,
        //     Mode = SpiMode.Mode0,  
        //     ChipSelectLineActiveState = 1,
        //     DataBitLength = 8
        // };
        //
        // // Create SPI device
        // var ads1256 = SpiDevice.Create(settings);
        //
        // // Send a command and read device ID
        // var adcDeviceId = getSpiDeviceId(ads1256);
        
        Console.WriteLine($"Starting ADS1256 reads on device ID={adcDeviceId} :");
        
        // var countdown = 500;
        //
        // while (countdown-- > 0)
        // {
        //     var pin00AnalogValue = readAdcSingleChannel(0, ads1256);
        //     var pin01AnalogValue = readAdcSingleChannel(1, ads1256);
        //     var pin02AnalogValue = readAdcSingleChannel(2, ads1256);
        //     var pin03AnalogValue = readAdcSingleChannel(3, ads1256);
        //     
        //     Console.WriteLine($"DeviceID: {adcDeviceId} | [ADC 0] is currently {pin00AnalogValue} | " + 
        //                       $"[ADC 1] is currently {pin01AnalogValue} | " + 
        //                       $"[ADC 2] is currently {pin02AnalogValue} | " +
        //                       $"[ADC 3] is currently {pin03AnalogValue}");
        // }
        
        Console.WriteLine($"Finished looping Waveshare AD/DA HAT read");
    }

    private void initAds1256()
    { 
        // config.module_init() below
        
        // This is just settings what numbering convention is used for the GPIO pins
        //         GPIO.setmode(GPIO.BCM)
        
        // We probably don't need this? Maybe?
        //         GPIO.setwarnings(False)    
        
        if (_gpio is null) throw new NullReferenceException("_gpio is null");
        
        //         GPIO.setup(RST_PIN, GPIO.OUT)
        _gpio.OpenPin(_resetPin, PinMode.Output);

        //         GPIO.setup(CS_PIN, GPIO.OUT)
        _gpio.OpenPin(_chipSelectPin, PinMode.Output);
        
        //         GPIO.setup(DRDY_PIN, GPIO.IN, pull_up_down=GPIO.PUD_UP)
        _gpio.OpenPin(_dataReadyPin, PinMode.InputPullUp);
        
        
        
        
        // 0,0 verified in original code
        var settings = new SpiConnectionSettings(0, 0)
        {
            ClockFrequency = 20000,             // SPI.max_speed_hz = 20000 
            Mode = SpiMode.Mode1,               // SPI.mode = 0b01 (BUT TEST MODE 0 IF THIS DOESN'T WORK)  
            // ChipSelectLineActiveState = 1,   // Disabling because haven't seen in original python code yet 
            // DataBitLength = 8                // Same as above
        };
        
        // Create SPI device
        var ads1256 = SpiDevice.Create(settings);

        //     self.ADS1256_reset()
        resetAds1256();

        //     id = self.ADS1256_ReadChipID()
        getAds1256DeviceId(ads1256);

        //     if id == 3 :
        //         print("ID Read success  ")
        //     else:
        //         print("ID Read failed   ")
        //         return -1
        //     self.ADS1256_ConfigADC(ADS1256_GAIN_E['ADS1256_GAIN_1'], ADS1256_DRATE_E['ADS1256_30000SPS'])
        //     return 0
    }

    private void resetAds1256()
    {
        if (_gpio is null) throw new NullReferenceException("_gpio is null");
        
        _gpio.Write(_resetPin, PinValue.High);
        Thread.Sleep(200);
        _gpio.Write(_resetPin, PinValue.Low);
        Thread.Sleep(200);
        _gpio.Write(_resetPin, PinValue.High);

        // def ADS1256_reset(self):
        //     config.digital_write(self.rst_pin, GPIO.HIGH)
        //     config.delay_ms(200)
        //     config.digital_write(self.rst_pin, GPIO.LOW)
        //     config.delay_ms(200)
        //     config.digital_write(self.rst_pin, GPIO.HIGH) 
    }

    public static int ConvertPinString(string pinToWorkText)
    {
        var pinInt = int.Parse(pinToWorkText);

        if (pinInt < 1) throw new ArgumentOutOfRangeException(nameof(pinInt), "While trying to convert pin string, pin seems invalid");

        if (pinInt > 40) throw new ArgumentOutOfRangeException(nameof(pinInt), "While trying to convert pin string, pin seems invalid");

        return pinInt;
    }
    
    private void getAds1256DeviceId(SpiDevice ads1256)
    {
        // def ADS1256_ReadChipID(self):
        //     self.ADS1256_WaitDRDY()
        waitForDataReadyAds1256();
        
        //     id = self.ADS1256_Read_data(REG_E['REG_STATUS'])
        var returnedData = readDataAds1256(ads1256, 0); // 0 is REG_E 'REG_STATUS"

        Console.WriteLine($"Device ID returned: {returnedData}");
        
        //     id = id[0] >> 4
        //     # print 'ID',id
        //     
        //     return id
        
        // Command to read the device ID from the STATUS register
        // Span<byte> writeBuffer = [0x10, 0x01];      // RREG command, STATUS register, 1 byte to read
        // Span<byte> readBuffer = stackalloc byte[2];
        //
        // // Send command and receive response
        // ads1256.TransferFullDuplex(writeBuffer, readBuffer);
        //
        // var deviceId = readBuffer[0];
        // return (int)deviceId;
    }

    private int readDataAds1256(SpiDevice ads1256, int reg)
    {
        if (_gpio is null) throw new NullReferenceException("_gpio is null");
        
        // def ADS1256_Read_data(self, reg):
        //     config.digital_write(self.cs_pin, GPIO.LOW)#cs  0
        _gpio.Write(_chipSelectPin, PinValue.Low);

        var rReg = 0x10;
        
        //     config.spi_writebyte([CMD['CMD_RREG'] | reg, 0x00])
        
        Span<byte> writeBuffer = [0x10, 0x00];      // RREG command, STATUS register, 1 byte to read
        ads1256.Write(writeBuffer);
        
        //     data = config.spi_readbytes(1)
        Span<byte> readBuffer = stackalloc byte[1];
        ads1256.Read(readBuffer);
        
        //     config.digital_write(self.cs_pin, GPIO.HIGH)#cs 1
        _gpio.Write(_chipSelectPin, PinValue.High);

        return readBuffer[0];
    }

    private void waitForDataReadyAds1256()
    {
        if (_gpio is null) throw new NullReferenceException("_gpio is null");
        
        for (var i = 0; i < 400000; i++)
        {
            if (_gpio.Read(_dataReadyPin) == 0)
            {
                break;
            }

            if (i >= 399999)
            {
                Console.WriteLine("Timed out waiting for data ready pin");
            }
        }
        
        // def ADS1256_WaitDRDY(self):
        //     for i in range(0,400000,1):
        //     if(config.digital_read(self.drdy_pin) == 0):
        //             
        //     break
        //     if(i >= 400000):
        //     print ("Time Out ...\r\n")
    }

    private int readAdcSingleChannel(byte channel, SpiDevice ads1256)
    {
        // _gpio ??= new GpioController();
        //
        // _gpio.OpenPin(_dataReadyPin, PinMode.Output);
        //
        // // 1. Send the RDATAC command to exit continuous mode (if active)
        // ads1256.Write(new byte[] { 0x01 });
        //
        // // 2. Write the MUX register to select the desired channel
        // byte muxRegCommand = (byte)(0x50 | 0x01); // WREG command for MUX (0x01)
        // byte muxValue = (byte)((channel << 4) | 0x08); // INP = channel, INN = AINCOM
        // ads1256.Write(new byte[] { muxRegCommand, 0x00, muxValue });
        //
        // // 3. Send the SYNC command
        // ads1256.Write(new byte[] { 0x04 });
        //
        // // 4. Send the WAKEUP command
        // ads1256.Write(new byte[] { 0x00 });
        // Thread.Sleep(10); // Settle time
        //
        // // 5. Wait for DRDY pin to go low (indicating data is ready)
        // while (_gpio.Read(_dataReadyPin) == PinValue.High) { }
        //
        // // 6. Send the RDATA command
        // Span<byte> writeBuffer = stackalloc byte[3] { 0x10, 0x0, 0x0 };
        // Span<byte> readBuffer = stackalloc byte[3]; // Read 3 bytes for 24-bit data
        // ads1256.TransferFullDuplex(writeBuffer, readBuffer);
        //
        // // 7. Combine the 3 bytes into a single integer
        // int result = (readBuffer[0] << 16) | (readBuffer[1] << 8) | readBuffer[2];
        //
        // // Handle the 24-bit signed representation
        // if ((result & 0x800000) != 0)
        // {
        //     result |= ~0xFFFFFF;
        // }
        // return result;
        
        throw new NotImplementedException();
    }
}