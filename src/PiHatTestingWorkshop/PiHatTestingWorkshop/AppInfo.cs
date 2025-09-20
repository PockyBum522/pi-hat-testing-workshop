using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PiHatTestingWorkshop;

public static class AppInfo
{
    public const string AppName = "PiHatTestingWorkshop";

    public static class Paths
    {
        static Paths()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var basePath = @"C:\Users\Public\Documents";

                basePath = Path.Combine(basePath, AppName);

                setAllPaths(basePath);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var basePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), AppName);

                Debug.WriteLine($"BASE PATH IS: {basePath}");

                setAllPaths(basePath);
            }

            if (string.IsNullOrWhiteSpace(ApplicationLoggingDirectory))
            {
                throw new Exception("OS Could not be detected automatically");
            }
        }

        private static void setAllPaths(string basePath)
        {
            var logBasePath = Path.Join(basePath, "logs");
            
            ApplicationLoggingDirectory = Path.Join(logBasePath, "application");
            
            Directory.CreateDirectory(ApplicationLoggingDirectory);
        }

        public static string ApplicationLoggingDirectory { get; private set; }
    }
}