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

        if (runAsGuiArgumentFound)
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
        
        // TODO: Handle arguments and actually running corresponding piHardwareHandler methods
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
