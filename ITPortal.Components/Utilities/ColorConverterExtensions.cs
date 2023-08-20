using System.Drawing;

namespace ITPortal.Components.Utilities;

public static class ColorConverterExtensions
{
    public static string ToHexString(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}";

    public static string ToRgbString(this Color c) => $"RGB({c.R}, {c.G}, {c.B})";

    public static string ToArgbString(this Color c) => $"ARGB({c.A}, {c.R}, {c.G}, {c.B})";

    public static Color AddTransparencyToHtmlColor(string htmlColor, int alpha)
    {
        return Color.FromArgb(alpha, ColorTranslator.FromHtml(htmlColor));
    }
}
