namespace PiHatTestingWorkshop;

public static class ApplicationPaths
{
    static ApplicationPaths()
    {
        var basePath = "";
        
        if (Environment.UserName == "david")
        {
            // Hardcoding my path here
            basePath = "/media/secondary/repos/linux-files/configuration/dotfiles/";
        }
        else
        {
            // This handles both Linux and Windows
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
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
        
        UserSettingsDirectory = Path.Join(basePath, "morse-practice");
    }
    
    public static string ApplicationLoggingDirectory { get; private set; }
    
    public static string UserSettingsDirectory { get; private set; }
}