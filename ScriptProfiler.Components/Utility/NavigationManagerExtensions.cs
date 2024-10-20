using Microsoft.AspNetCore.Components;

namespace ScriptProfiler.Components.Utility;

public static class NavigationManagerExtensions
{
    public static string GetPage(this NavigationManager navigation)
    {
        return navigation.Uri[(navigation.BaseUri.Length - 1)..];
    }
}
