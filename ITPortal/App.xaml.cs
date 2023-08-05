using ITPortal.Lib.Utils;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
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
            appWindow.Resize(new SizeInt32(DeviceWindowHandler.WindowWidth, DeviceWindowHandler.WindowHeight));
#endif
        });

            MainPage = new MainPage();
    }
}
