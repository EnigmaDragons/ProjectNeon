using I2.Loc;

public static class TermExtensions
{
    public static string ToEnglish(this string term) => LocalizationManager.GetTranslation(term, overrideLanguage: "English");
    public static string ToLocalized(this string term) => LocalizationManager.GetTranslation(term);
}