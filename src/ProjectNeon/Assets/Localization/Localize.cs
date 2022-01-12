using UnityEngine.Localization.Settings;

public static class Localize
{   
    private static LocalizedStringDatabase Db => _db ??= LocalizationSettings.StringDatabase;
    private static LocalizedStringDatabase _db;
    
    public static string GetUI(string key) => Get("UI", key);
    public static string GetEvent(string key) => Get("Events", key);
    public static string GetEventResult(string key) => Get("EventResults", key);
    public static string GetFormattedEventResult(string key, params object[] args) => GetFormatted("EventResults", key, args);
    public static string GetStat(string key) => Get("Stats", key);
    public static string GetStat(StatType key) => Get("Stats", key.ToString());
    public static string GetHero(string key) => Get("Heroes", key);

    public static void SetDb(LocalizedStringDatabase db) => _db = db;
    
    public static string GetFormatted(string sheet, string key, params object[] args)
    {
        return Db.GetLocalizedString(sheet, key, args);
    }
    
    public static string Get(string sheet, string key)
    {
        return GetLocalizedStringOrDefault(sheet, key);
    }
    
    private static string GetLocalizedStringOrDefault(string sheet, string key)
    {
        var localized = Db.GetLocalizedString(sheet, key);
        return string.IsNullOrWhiteSpace(localized) ? key : localized;
    }
}
