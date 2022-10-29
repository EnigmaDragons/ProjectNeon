#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LocalizationExporter
{
    private static string BaseDir = ".\\LocalizationAssets";
    private static string LocalizeNewLineSymbol = "{[BR]}";

    private static void WriteCsv(string filename, List<string> data)
    {
        if (!Directory.Exists(BaseDir))
            Directory.CreateDirectory(BaseDir);
        
        File.WriteAllLines($"{BaseDir}\\{filename}.csv", data);
    }
    
    [MenuItem("Neon/Localization/ExportCardsForLocalization")]
    public static void ExportCardsForLocalization()
    {
        var allCards = GetAllInstances<CardType>()
            .Where(c => !c.IsWip)
            .OrderBy(c => c.Id);

        var data = new List<string>();
        allCards.ForEach(c =>
        {
            data.Add($"{c.CardLocalizationNameKey()}^{c.Name}");
            var csvDesc = c.description
                .Replace(Environment.NewLine, LocalizeNewLineSymbol)
                .Replace("\r\n", LocalizeNewLineSymbol)
                .Replace("\n", LocalizeNewLineSymbol);
            data.Add($"{c.CardLocalizationDescriptionKey()}^{csvDesc}");
        });
        WriteCsv("cards-for-localization", data);
    }

    [MenuItem("Neon/Localization/ExportArchetypesForLocalization")]
    public static void ExportArchetypesForLocalization()
    {
        var allArchetypes = GetAllInstances<StringVariable>()
            .Where(x => x.name.ContainsAnyCase("Archetype"))
            .Select(x => x.Value)
            .OrderBy(x => x)
            .ToList();
        
        WriteCsv("archetypes", allArchetypes);
    }

    [MenuItem("Neon/Localization/ExportKeywordRules")]
    public static void ExportKeywordRules()
    {
        GetAllInstances<StringKeyValueCollection>()
            .Where(c => c.name.Equals("KeywordRules"))
            .FirstAsMaybe()
            .IfPresent(keywordRules =>
            {
                var data = new List<string>();
                foreach (var k in keywordRules.All)
                {
                    data.Add($"{k.Key.Value}^{k.Key.Value}");
                    var csvDesc = k.Value
                        .Replace(Environment.NewLine, LocalizeNewLineSymbol)
                        .Replace("\r\n", LocalizeNewLineSymbol)
                        .Replace("\n", LocalizeNewLineSymbol);
                    data.Add($"{k.Key.Value}_Rule^{csvDesc}");
                }

                WriteCsv("keywords", data);
            });
    }

    [MenuItem("Neon/Localization/ExportTutorialSlides")]
    public static void ExportTutorialSlides()
    {
        var data = new List<string>();
        GetAllInstances<TutorialSlide>().ForEach(s =>
        {
            var text = s.Text
                .Replace(Environment.NewLine, LocalizeNewLineSymbol)
                .Replace("\r\n", LocalizeNewLineSymbol)
                .Replace("\n", LocalizeNewLineSymbol);
            data.Add($"Tutorial_Slide_{s.id}^{text}");
        });
        WriteCsv("tutorial-slides", data);
    }

    private static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        var guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);
        var a = new T[guids.Length];
        for(int i =0; i<guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }
 
        return a;
    }
}

#endif