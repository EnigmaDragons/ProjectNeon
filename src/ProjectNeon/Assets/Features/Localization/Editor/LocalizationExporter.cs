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

    private static void WriteCsv(string filename, List<string> data)
    {
        if (!Directory.Exists(BaseDir))
            Directory.CreateDirectory(BaseDir);
        
        File.WriteAllLines($"{BaseDir}\\{filename}.csv", data);
    }

    private static string ToSingleLineI2Format(string toReplace)
        => toReplace.ToI2Format()
            .Replace("\r\n", "<br>")
            .Replace("\n", "<br>");
    
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
            var csvDesc = ToSingleLineI2Format(c.description);
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
                    var csvDesc = ToSingleLineI2Format(k.Value);
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
            var text = ToSingleLineI2Format(s.Text);
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
                    data.Add($"Segment{segment.Id}^{ToSingleLineI2Format(segment.Text)}");
        WriteCsv("cutscenes", data);
    }

    [MenuItem("Neon/Localization/Export Loading Screens")]
    public static void ExportLoadingScreens()
    {
        var data = new List<string>();
        foreach (var screen in GetAllInstances<CorpLoadingScreen>())
            data.Add($"LocationTitle{screen.id}^{ToSingleLineI2Format(screen.locationTitle)}");
        WriteCsv("loading-screens", data);
    }

    [MenuItem("Neon/Localization/Export Hero Names")]
    public static void ExportHeroNames()
    {
        var data = new List<string>();
        foreach (var hero in GetAllInstances<BaseHero>())
            data.Add($"HeroName{hero.id}^{hero.name}");
        WriteCsv("hero-names", data);
    }

    [MenuItem("Neon/Localization/Export Enemy Descriptions")]
    public static void ExportEnemyDescriptions()
    {
        var data = new List<string>();
        foreach (var enemy in GetAllInstances<Enemy>())
            data.Add($"EnemyDescription{enemy.id}^{enemy.description}");
        WriteCsv("enemy-descriptions", data);
    }

    [MenuItem("Neon/Localization/Export Hero Classes")]
    public static void ExportHeroClasses()
    {
        var data = new List<string>();
        foreach (var hero in GetAllInstances<BaseHero>())
            data.Add($"HeroClass{hero.id}^{hero.className}");
        WriteCsv("hero-classes", data);
    }

    [MenuItem("Neon/Localization/Export Hero Flavor")]
    public static void ExportHeroFlavor()
    {
        var data = new List<string>();
        foreach (var hero in GetAllInstances<BaseHero>())
        {
            data.Add($"HeroDescription{hero.id}^{hero.flavorDetails.HeroDescription}");  
            data.Add($"HeroBackStory{hero.id}^{hero.flavorDetails.BackStory}"); 
        }
        WriteCsv("hero-flavor", data);
    }

    [MenuItem("Neon/Localization/Export Blessings")]
    public static void ExportBlessings()
    {
        var data = new List<string>();
        foreach (var provider in GetAllInstances<CorpClinicProvider>())
        {
            foreach (var blessing in provider.blessingsV4)
            {
                data.Add($"Blessing{blessing.Id}Name^{blessing.Name}");
                data.Add($"Blessing{blessing.Id}Description^{blessing.Description}");
            }   
        }
        WriteCsv("blessings", data);
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