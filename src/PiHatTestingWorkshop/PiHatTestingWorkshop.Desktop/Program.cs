using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using PiHatTestingWorkshop.PiHardware;
using PiHatTestingWorkshop.ViewModels;
using PiHatTestingWorkshop.Views;
using Serilog;

namespace PiHatTestingWorkshop.Desktop;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task Main(string[] args)
    {
        // Check arguments because we don't need to build the UI if we're running as -nogui
        var argsCounter = 0;

        var runAsGuiArgumentFound = false;
        
        foreach (var argument in Environment.GetCommandLineArgs())
        {
            Console.WriteLine($"[CLI Argument {argsCounter}] = '{argument}' (Without single quotes)");
            
            argsCounter++;

            if (argument.ToLower().Contains("-startgui"))
                runAsGuiArgumentFound = true;
        }

        if (!runAsGuiArgumentFound)
        {
            await runApplicationAsCliOnly();
        }
        else
        {
            // If we weren't explicitly told to run the gui then don't
            return;
        }
        
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            DependencyInjectionRoot.LoggerApplication.Warning(ex, "An error occurred while starting the application");
        }
        finally
        {
            var counter = 20;
        
            while (counter-- > 0)
            {
                Log.CloseAndFlush();
                
                Thread.Sleep(100);
            }
        }
    }

    private static async Task runApplicationAsCliOnly()
    {
        var dependencyContainer = DependencyInjectionRoot.GetBuiltContainer();
        
        await using var scope = dependencyContainer.BeginLifetimeScope();
        
        var logger = scope.Resolve<ILogger>();
        
        if (logger is null) throw new NullReferenceException("_logger was null in App.axaml.cs");
        
        logger.Information("Application started. User has chosen CLI only");

        var piHardwareHandler = scope.Resolve<PiHardwareHandler>();
        
        // NOTE: If you add arguments below, add a corresponding way to run that test in MainViewModel.cs
        
        for (var i = 0; i < Environment.GetCommandLineArgs().Length; i++)
        {
            var argument = Environment.GetCommandLineArgs()[i];

            if (argument.Contains("-help", StringComparison.CurrentCultureIgnoreCase) ||
                argument.Contains("-?", StringComparison.CurrentCultureIgnoreCase) ||
                argument.Equals("-h", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine();
                Console.WriteLine("Raspberry Pi HAT Testing Workshop:");
                Console.WriteLine();
                Console.WriteLine("    Options:");
                Console.WriteLine("        -setPinHigh [Pin Number] (Example: -setPinHigh 1)");
                Console.WriteLine("            Pin number is required. Sets the specified GPIO pin to HIGH (3.3v)");
                Console.WriteLine();
                Console.WriteLine("        -setPinLow [Pin Number] (Example: -setPinLow 1)");
                Console.WriteLine("            Pin number is required. Sets the specified GPIO pin to LOW (0v)");
                Console.WriteLine();
                Console.WriteLine("        -readPin [Pin Number] (Example: -readPin 1)");
                Console.WriteLine("            Pin number is required. Reads the specified GPIO pin several hundred times in a row");
                Console.WriteLine();
                Console.WriteLine("        -readAdDaHat");
                Console.WriteLine("            Reads the first three channels on a Waveshare High-Precision AD/DA HAT several hundred times in a row");
                Console.WriteLine();
                Console.WriteLine();
            }
            
            if (argument.ToLower().Contains("-setpinhigh"))
            {
                // Call pihardware set pin here and convert next arg to int
                throw new NotImplementedException();
            }
            
            if (argument.ToLower().Contains("-setpinlow"))
            {
                // Call pihardware set pin here and convert next arg to int
                throw new NotImplementedException();
            }
            
            if (argument.ToLower().Contains("-readpin"))
            {
                // Call pihardware read pin here and convert next arg to int
                throw new NotImplementedException();
            }
            
            if (argument.ToLower().Contains("-readaddahat"))
            {
                piHardwareHandler.ReadWaveshareAdDaHatRepeatedly();
            }
        }
        
        // NOTE: If you add arguments above, add a corresponding way to run that test in MainViewModel.cs
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
