#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Application = Microsoft.Maui.Controls.Application;
#endif

namespace ITPortal;

public partial class App : Application
{
    public App()
	{
		InitializeComponent();

        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
#if WINDOWS
            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();

            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            DeviceWindowHandler.WindowHandle = windowHandle;

            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            // Get screen size minus size of the taskbar
            int screenWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;

            int windowWidth = 100 + screenWidth / 2;
            int windowHeight = 150 + screenHeight / 2;

            int centerX = screenWidth / 2 - windowWidth / 2;
            int centerY = screenHeight / 2 - windowHeight / 2;

            appWindow.Resize(new SizeInt32(windowWidth, windowHeight));
            appWindow.Move(new PointInt32(centerX, centerY));
#endif
        });
        
        MainPage = new MainPage();
    }
}
