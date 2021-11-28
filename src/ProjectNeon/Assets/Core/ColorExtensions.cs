using UnityEngine;

public static class ColorExtensions
{
    public static Color WithAlpha(this Color baseColor, float alpha) =>
        new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

    public static Color Transparent(this Color baseColor) 
        => baseColor.WithAlpha(0);
}
