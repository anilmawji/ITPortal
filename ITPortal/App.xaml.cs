using ITPortal.Lib.Utils;

namespace ITPortal;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

#if WINDOWS
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();
            DeviceWindowHandler.WindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
    });
#endif

        MainPage = new MainPage();
    }
}
