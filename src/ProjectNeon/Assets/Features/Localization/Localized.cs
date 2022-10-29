using I2.Loc;

public static class Localized
{
    public static string Archetype(string term) => String("Archetypes", term);
    public static string String(string category, string term) => new LocalizedString($"{category}/{term}").ToString();
}
