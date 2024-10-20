using System.Drawing;

namespace ScriptProfiler.Components.Utility;

public static class ColorHelper
{
    public static string ToHexString(this System.Drawing.Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}";

    public static string ToRgbString(this System.Drawing.Color c) => $"RGB({c.R}, {c.G}, {c.B})";

    public static string ToArgbString(this System.Drawing.Color c) => $"ARGB({c.A}, {c.R}, {c.G}, {c.B})";

    public static System.Drawing.Color AddTransparencyToHtmlColor(string htmlColor, int alpha)
    {
        return System.Drawing.Color.FromArgb(alpha, ColorTranslator.FromHtml(htmlColor));
    }
}
