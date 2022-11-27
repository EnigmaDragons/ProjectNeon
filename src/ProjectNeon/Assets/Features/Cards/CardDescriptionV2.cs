using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class CardDescriptionV2
{
    [SerializeField] [TextArea(1, 12)] public string text = string.Empty;
    public string[] formatArgs = Array.Empty<string>();

    public string Preview()
    {
        try
        {
            return text != null ? string.Format(text, formatArgs) : "";
        }
        catch (Exception)
        {
            return $"Invalid Format Pattern: {text}";
        }
    }
    
    public bool IsUsable() => !string.IsNullOrWhiteSpace(text);
    public string ToI2Format() => text.ToI2Format();
    public string ToSingleLineI2Format() => text.ToSingleLineI2Format();
    
    public static CardDescriptionV2 FromDescriptionV1(string descriptionV1)
    {
        var argsList = new List<string>();
        var descV2 = descriptionV1;
        var tokens = Regex.Matches(descV2, "{(.*?)}");
        for (var i = 0; i < tokens.Count; i++)
        {
            var t = tokens[i];
            argsList.Add(t.Value);
            descV2 = descV2.Replace(t.Value, "{" + i + "}");
        }
        
        return new CardDescriptionV2 { text = descV2, formatArgs = argsList.ToArray() };
    }
    
    //
    // foreach (var r in _resourceIcons)
    //     result = Regex.Replace(result, $" {r.Key}", $" {Sprite(r.Value)}");
    //
    // result = Regex.Replace(result, @"@(\w+)", "$1");
}
