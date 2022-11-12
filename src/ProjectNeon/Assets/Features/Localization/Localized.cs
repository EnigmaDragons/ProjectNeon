using System;
using System.Collections.Generic;
using I2.Loc;

public static class Localized
{

    public static string ToEnglish(this string term) => LocalizationManager.GetTranslation(term, overrideLanguage: "English") ?? "";
    public static string ToLocalized(this string term) => StringTerm(term);
    
    public static string Archetype(string term) => String("Archetypes", term);

    public static string String(string category, string term) => StringTerm($"{category}/{term}");
    public static string StringTerm(string term)
    {
        var result = new LocalizedString(term).ToString();
        return string.IsNullOrWhiteSpace(result) ? term.ToEnglish() : result;
    }

    public static string StringTermOrDefault(string term, string def)
    {
        try
        {
            return new LocalizedString(term).ToString();
        }
        catch (Exception e)
        {
            return def;
        }
    }

    public static string Format(string category, string term, params object[] args)
        => FormatTerm($"{category}/{term}", args);
    
    public static string FormatTerm(string term, params object[] args)
        => Format(StringTerm(term), args);

    public static string FormatTermOrDefault(string term, string defaultTranslation, params object[] args)
    {
        try
        {
            return Format(StringTermOrDefault(term, defaultTranslation), args);
        }
        catch (Exception)
        {
            return Format(defaultTranslation, args);
        }
    }

    private static string Format(string template, params object[] args)
    {
        try
        {
            return string.Format(template, args);
        }
        catch (Exception)
        {
            Log.Error($"Unable to Format. Input String {template}");
            throw;
        }
    }
    
    public static string ToI2Format(this string s)
    {
        var r = s;
        GlobalParams.Backward.ForEach(p => r = r.Replace(p.Key, "{[" + p.Value + "]}"));
        return r;
    }
    
    public static string FromI2Format(this string s)
    {
        var r = s;
        GlobalParams.Backward.ForEach(p => r = r.Replace("{[" + p.Value + "]}", p.Key));
        return r;
    }

    public static string FromI2ParamValue(this string paramName)
    {
        return GlobalParams.Forward.ValueOrMaybe(paramName).OrDefault((string)null);
    }

    private static readonly BidirectionalDictionary<string, string> GlobalParams = new BidirectionalDictionary<string, string>(
        new Dictionary<string, string>
        {
            { "BR", "<br>" },
            { "B", "<b>" },
            { "/B", "</b>" },
            { "I", "<i>" },
            { "/I", "</i>" },
            { "0", "{0}" },
            { "1", "{1}" },
            { "2", "{2}" },
            { "3", "{3}" },
            { "4", "{4}" },
            { "5", "{5}" },
            { "6", "{6}" },
            { "7", "{7}" },
            { "8", "{8}" },
            { "9", "{9}" },
        });
}
