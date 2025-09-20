using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PiHatTestingWorkshop.Views;

namespace PiHatTestingWorkshop;

public class App : Application
{
    private static readonly string _userPreferencesFileName = $".{Environment.MachineName}-pi-hat-testing-workshop.json";
    
    public static readonly string UserPreferencesFullPath = Path.Combine(ApplicationPaths.UserSettingsDirectory, _userPreferencesFileName);
    
    private static ILogger? _logger;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var dependencyContainer = DependencyInjectionRoot.GetBuiltContainer(ApplicationUiModeEnum.Gui);
        
        await using var scope = dependencyContainer.BeginLifetimeScope();
        
        _logger = scope.Resolve<ILogger>();
        
        if (_logger is null) throw new NullReferenceException("_logger was null in App.axaml.cs");
        
        _logger.Information("Application started. About to fire up MainWindow if IClassicDesktopStyleApplicationLifetime or MainView if ISingleViewApplicationLifetime");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = scope.Resolve<MainWindow>();
            mainWindow.Content = scope.Resolve<MainView>();
        
            desktop.MainWindow = mainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = scope.Resolve<MainView>();
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    // private static List<SavedWindowPreferences> loadJsonSavedConfiguration(string configurationFilePath)
    // {
    //     if (_logger is null) throw new NullReferenceException($"_logger was null in {nameof(loadJsonSavedConfiguration)}"); 
    //     
    //     _logger.Information("Loading json saved configuration from: {Path}", configurationFilePath);
    //     
    //     var jsonString = File.ReadAllText(configurationFilePath);
    //     
    //     var returnWindowPreferences = JsonConvert.DeserializeObject<List<SavedWindowPreferences>>(jsonString);
    //
    //     _logger.Debug("Checking if loaded anything for returnWindowPreferences: {@ReturnPrefs}", returnWindowPreferences);
    //     
    //     return returnWindowPreferences ?? [];
    // }
    //
    // private static void saveExampleWindowStateConfig()
    // {
    //     var listToSave = new List<SavedWindowPreferences>();
    //
    //     var dummyWindow01 = new SavedWindowPreferences();
    //     
    //     dummyWindow01.TitlePattern = "Title Pattern Dummy Window 01 Title";
    //     dummyWindow01.ClassPattern = "Class Pattern Dummy Window 01 Class";
    //     
    //     dummyWindow01.LeftTopScalingMultiple = 2.0m;
    //     
    //     dummyWindow01.PreferredPositions.Add(
    //         new WindowPosition(200, 200, 10, 20));
    //     
    //     dummyWindow01.PreferredPositions.Add(
    //         new WindowPosition(400, 400, 10, 20));
    //     
    //     listToSave.Add(dummyWindow01);
    //     
    //     var dummyWindow02 = new SavedWindowPreferences();
    //     
    //     dummyWindow02.TitlePattern = "Title Pattern Dummy Window 02 Title";
    //     dummyWindow02.ClassPattern = "Class Pattern Dummy Window 02 Class";
    //     
    //     dummyWindow02.PreferredPositions.Add(
    //         new WindowPosition(200, 200, 10, 20));
    //     
    //     dummyWindow02.PreferredPositions.Add(
    //         new WindowPosition(400, 400, 10, 20));
    //     
    //     listToSave.Add(dummyWindow02);
    //
    //     var windowInformationFilePath = UserPreferencesFullPath;
    //     
    //     var windowJson = JsonConvert.SerializeObject(listToSave, Formatting.Indented);
    //
    //     _logger?.Information("Saving example config file to: {WindowInformationFilePath}", windowInformationFilePath);
    //     
    //     File.WriteAllText(windowInformationFilePath, windowJson);
    // }
}

