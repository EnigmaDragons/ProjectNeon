using UnityEngine.Localization.Settings;

public static class Localize
{
    public static string GetUI(string key) => Get("UI", key);
    public static string GetEvent(string key) => Get("Events", key);
    public static string GetEventResult(string key) => Get("EventResults", key);
    public static string GetFormattedEventResult(string key, params object[] args) => GetFormatted("EventResults", key, args);
    public static string GetStat(string key) => Get("Stats", key);
    public static string GetStat(StatType key) => Get("Stats", key.ToString());
    public static string GetHero(string key) => Get("Heroes", key);

    public static string GetFormatted(string sheet, string key, params object[] args)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(sheet, key, args);
    }
    public static string Get(string sheet, string key) => LocalizationSettings.StringDatabase.GetLocalizedString(sheet, key);
}