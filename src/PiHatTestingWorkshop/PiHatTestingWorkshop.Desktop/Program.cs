using System;
using System.Diagnostics;
using System.Threading;
using Avalonia;
using Avalonia.ReactiveUI;
using Serilog;

namespace PiHatTestingWorkshop.Desktop;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Check arguments because we don't need to build the UI if we're running as -no-gui
        var argsCounter = 0;
        
        foreach (var argument in Environment.GetCommandLineArgs())
        {
            Console.WriteLine($"[CLI Argument {argsCounter}] = '{argument}' (Without single quotes)");
            
            argsCounter++;
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

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
