using System.Collections.Generic;
using I2.Loc;

public static class Localized
{
    public static string Archetype(string term) => String("Archetypes", term);
    public static string String(string category, string term) => new LocalizedString($"{category}/{term}").ToString();

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
