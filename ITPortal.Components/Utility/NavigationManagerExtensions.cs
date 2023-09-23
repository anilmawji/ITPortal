using Microsoft.AspNetCore.Components;

namespace ITPortal.Components.Utility;

public static class NavigationManagerExtensions
{
    public static string GetPage(this NavigationManager navigation)
    {
        return navigation.Uri.Substring(navigation.BaseUri.Length - 1);
    }
}
