using ScriptProfiler;

namespace ScriptProfiler;

public sealed partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        //#if WINDOWS
        //        // Get screen size excluding size of the taskbar
        //        int screenWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
        //        int screenHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;

        //        int defaultWidth = 100 + screenWidth / 2;
        //        int defaultHeight = 150 + screenHeight / 2;
        //#else
        //        int screenWidth = 1080;
        //        int screenHeight = 740;

        //        int defaultWidth = screenWidth;
        //        int defaultHeight = screenHeight;
        //#endif
        //        int centerX = screenWidth / 2 - defaultWidth / 2;
        //        int centerY = screenHeight / 2 - defaultHeight / 2;

        //        window.X = centerX;
        //        window.Y = centerY;

        //        window.MinimumWidth = 700;
        //        window.MinimumHeight = 500;

        //        window.Width = defaultWidth;
        //        window.Height = defaultHeight;

        return window;
    }
}
