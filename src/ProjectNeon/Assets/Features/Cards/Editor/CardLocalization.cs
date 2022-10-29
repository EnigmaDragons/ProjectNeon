#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardLocalization
{
    private static string BaseDir = ".\\LocalizationAssets";

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

        var localizeNewLineSymbol = "{[BR]}";
        var data = new List<string>();
        allCards.ForEach(c =>
        {
            data.Add($"{c.CardLocalizationNameKey()}^{c.Name}");
            var csvDesc = c.description
                .Replace(Environment.NewLine, localizeNewLineSymbol)
                .Replace("\r\n", localizeNewLineSymbol)
                .Replace("\n", localizeNewLineSymbol);
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