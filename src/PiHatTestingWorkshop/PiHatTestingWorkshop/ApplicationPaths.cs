namespace PiHatTestingWorkshop;

public static class ApplicationPaths
{
    static ApplicationPaths()
    {
        var basePath = "";
        
        if (Environment.UserName == "david")
        {
            // Hardcoding my path here
            basePath = "/media/secondary/repos/pi-hat-testing-workshop/configuration/dotfiles/";
        }
        else
        {
            // This handles both Linux and Windows
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
        
        // Make sure it uh, exists
        if (!Directory.Exists(basePath))
        {
            Console.WriteLine("Could not find path: " + basePath);
            
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            
            Console.WriteLine($"Using: '{basePath}' instead");
        }
        
        setAllPaths(basePath);
        
        if (string.IsNullOrWhiteSpace(ApplicationLoggingDirectory) ||
            string.IsNullOrWhiteSpace(UserSettingsDirectory))
        {
            throw new Exception("User profile folder path could not be detected automatically");
        }
        
        Directory.CreateDirectory(ApplicationLoggingDirectory);
        Directory.CreateDirectory(UserSettingsDirectory);
    }

    private static void setAllPaths(string basePath)
    {
        var logBasePath = Path.Join(basePath, "Logs");
            
        ApplicationLoggingDirectory = Path.Join(logBasePath, "Logs");
        
        UserSettingsDirectory = Path.Join(basePath, "pi-hat-testing-workshop");
    }
    
    public static string ApplicationLoggingDirectory { get; private set; }
    
    public static string UserSettingsDirectory { get; private set; }
}