namespace PiHatTestingWorkshop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        initializeWindowPropertiesForDesktop();

        InitializeComponent();
    }

    private bool argumentFound(string needle, string[] args)
    {
        foreach (var argument in args)
        {
            if (argument.Contains(needle)) return true;
        }

        return false;
    }

    private void initializeWindowPropertiesForDesktop()
    {
        WindowState = WindowState.Normal;
        
        CanResize = true;
        
        ExtendClientAreaToDecorationsHint = false;

        //ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
        
        ExtendClientAreaTitleBarHeightHint = 1;
    }
}