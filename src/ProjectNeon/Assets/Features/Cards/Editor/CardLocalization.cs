#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardLocalization
{
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
        File.WriteAllLines("c:\\temp\\cards-for-localization.csv", data);
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