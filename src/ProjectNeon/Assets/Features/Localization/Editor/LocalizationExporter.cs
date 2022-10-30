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
    private static string LocalizeOpenBold = "{[B]}";
    private static string LocalizeCloseBold = "{[/B]}";

    private static void WriteCsv(string filename, List<string> data)
    {
        if (!Directory.Exists(BaseDir))
            Directory.CreateDirectory(BaseDir);
        
        File.WriteAllLines($"{BaseDir}\\{filename}.csv", data);
    }

    private static string ReplaceSpecialCharacters(string toReplace)
        => toReplace
            .Replace(Environment.NewLine, LocalizeNewLineSymbol)
            .Replace("\r\n", LocalizeNewLineSymbol)
            .Replace("\n", LocalizeNewLineSymbol)
            .Replace("<b>", LocalizeOpenBold)
            .Replace("</b>", LocalizeCloseBold);
    
    [MenuItem("Neon/Localization/Export Cards For Localization")]
    public static void ExportCardsForLocalization()
    {
        var allCards = GetAllInstances<CardType>()
            .Where(c => !c.IsWip)
            .OrderBy(c => c.Id);

        var data = new List<string>();
        allCards.ForEach(c =>
        {
            data.Add($"{c.CardLocalizationNameKey()}^{c.Name}");
            var csvDesc = ReplaceSpecialCharacters(c.description);
            data.Add($"{c.CardLocalizationDescriptionKey()}^{csvDesc}");
        });
        WriteCsv("cards-for-localization", data);
    }

    [MenuItem("Neon/Localization/Export Archetypes For Localization")]
    public static void ExportArchetypesForLocalization()
    {
        var allArchetypes = GetAllInstances<StringVariable>()
            .Where(x => x.name.ContainsAnyCase("Archetype"))
            .Select(x => x.Value)
            .OrderBy(x => x)
            .ToList();
        
        WriteCsv("archetypes", allArchetypes);
    }

    [MenuItem("Neon/Localization/Export Keyword Rules")]
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
                    var csvDesc = ReplaceSpecialCharacters(k.Value);
                    data.Add($"{k.Key.Value}_Rule^{csvDesc}");
                }

                WriteCsv("keywords", data);
            });
    }

    [MenuItem("Neon/Localization/Export Tutorial Slides")]
    public static void ExportTutorialSlides()
    {
        var data = new List<string>();
        GetAllInstances<TutorialSlide>().ForEach(s =>
        {
            var text = ReplaceSpecialCharacters(s.Text);
            data.Add($"Tutorial_Slide_{s.id}^{text}");
        });
        WriteCsv("tutorial-slides", data);
    }

    [MenuItem("Neon/Localization/Export Cutscenes")]
    public static void ExportCutscenes()
    {
        var data = new List<string>();
        foreach(var cutscene in GetAllInstances<Cutscene>())
            foreach(var segment in cutscene.Segments)
                if (segment.SegmentType == CutsceneSegmentType.DialogueLine || segment.SegmentType == CutsceneSegmentType.NarratorLine || segment.SegmentType == CutsceneSegmentType.PlayerLine)
                    data.Add($"Segment{segment.Id}^{ReplaceSpecialCharacters(segment.Text)}");
        WriteCsv("cutscenes", data);
    }

    [MenuItem("Neon/Localization/Export Story Events")]
    public static void ExportStoryEvents()
    {
        var data = new List<string>();
        foreach (var e in GetAllInstances<StoryEvent2>())
        {
            data.Add($"Event{e.id}^{ReplaceSpecialCharacters(e.StoryText)}");
            foreach (var choice in e.Choices)
                data.Add($"Choice{choice.Id}^{ReplaceSpecialCharacters(choice.ChoiceText(e.id))}");   
        }
        WriteCsv("story-events", data);
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