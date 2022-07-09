
public static class TextColors
{
    public static string PhysDamageColored(this string s) => Colored(s, "#fac34c");
    public static string PhysDamageColoredDark(this string s) => Colored(s, "#7b2e00");
    public static string RawDamageColored(this string s) => Colored(s, "#ff647b");
    public static string MagicDamageColored(this string s) => Colored(s, "#d79fff");
    public static string MagicDamageColoredDark(this string s) => Colored(s, "#530e90");
    public static string Colored(this string s, string color) => $"<color={color}>{s}</color>";
}