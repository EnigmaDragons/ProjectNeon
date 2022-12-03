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

    public static void WriteCsv(string filename, List<string> data)
    {
        if (!Directory.Exists(BaseDir))
            Directory.CreateDirectory(BaseDir);
        
        File.WriteAllLines($"{BaseDir}\\{filename}.csv", data);
    }

    private static string ToSingleLineI2Format(string toReplace)
        => toReplace.ToSingleLineI2Format();
    
    [MenuItem("Neon/Localization/Export Cards For Localization")]
    public static void ExportCardsForLocalization()
    {
        var allCards = GetAllInstances<CardType>()
            .Where(c => !c.IsWip)
            .OrderBy(c => c.Id);

        var names = new List<string>();
        var descs = new List<string>();
        allCards.ForEach(c =>
        {
            names.Add($"{c.NameKey}^{c.Name}");
            var csvDesc = c.DescriptionV2.text.Replace("<br>", "\n").ToI2Format();
            descs.Add($"{c.DescriptionKey}^\"{csvDesc}\"");
        });
        WriteCsv("card-names", names);
        WriteCsv("card-descs", descs);
    }

    [MenuItem("Neon/Localization/Export Reaction Cards For Localization")]
    public static void ExportReactionCardsForLocalization()
    {
        var reactionCards = GetAllInstances<ReactionCardType>()
            .OrderBy(c => c.Id);
        
        var names = new List<string>();
        var descs = new List<string>();
        reactionCards.ForEach(c =>
        {
            names.Add($"{c.NameKey}^{c.Name}");
            var csvDesc = c.DescriptionV2.text.Replace("<br>", "\n").ToI2Format();
            descs.Add($"{c.DescriptionKey}^\"{csvDesc}\"");
        });
        WriteCsv("reaction-card-names", names);
        WriteCsv("reaction-card-descs", descs);
    }
    
    [MenuItem("Neon/Localization/Export Equipment For Localization")]
    public static void ExportEquipmentForLocalization()
    {
        var items = GetAllInstances<StaticEquipment>()
            .Where(x => !x.IsWip && x.Slot == EquipmentSlot.Augmentation)
            .OrderBy(x => x.Id);

        var names = new List<string>();
        var descs = new List<string>();
        items.ForEach(x =>
        {
            names.Add($"{x.LocalizationNameKey()}^{x.Name}");
            var csvDesc = ToSingleLineI2Format(x.Description);
            descs.Add($"{x.LocalizationDescriptionKey()}^{csvDesc}");
        });
        WriteCsv("augment-names", names);
        WriteCsv("augment-descs", descs);
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
                    data.Add($"{segment.Term.Split('/').Last()}^\"{segment.Text.Replace("<br>", "\n").ToI2Format()}\"");
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

    [MenuItem("Neon/Localization/Export Adventures")]
    public static void ExportAdventures()
    {
        var data = new List<string>();
        foreach (var adventure in GetAllInstances<Adventure>())
        {
            data.Add($"{adventure.TitleTerm.Split('/').Last()}^{ToSingleLineI2Format(adventure.adventureTitle ?? "")}");
            data.Add($"{adventure.RawMapTitleTerm.Split('/').Last()}^{ToSingleLineI2Format(adventure.mapAdventureTitle ?? "")}");
            data.Add($"{adventure.StoryTerm.Split('/').Last()}^{ToSingleLineI2Format(adventure.story ?? "")}");
            data.Add($"{adventure.AllowedHeroesDescriptionTerm.Split('/').Last()}^{ToSingleLineI2Format(adventure.allowedHeroesDescription ?? "")}");
            data.Add($"{adventure.LockConditionExplanationTerm.Split('/').Last()}^{ToSingleLineI2Format(adventure.lockConditionExplanation ?? "")}");
            data.Add($"{adventure.VictoryConclusionTerm.Split('/').Last()}^{ToSingleLineI2Format(adventure.victoryConclusion ?? "")}");
            data.Add($"{adventure.DefeatConclusionTerm.Split('/').Last()}^{ToSingleLineI2Format(adventure.defeatConclusion ?? "")}");
        }
        WriteCsv("adventures", data);
    }

    [MenuItem("Neon/Localization/Export Stat Types")]
    public static void ExportStatTypes()
    {
        var data = new List<string>();
        
        foreach (var x in Enum.GetValues(typeof(StatType)).Cast<StatType>())
            data.Add($"{x.GetShortTerm()}, {x}");
        foreach (var x in Enum.GetValues(typeof(TemporalStatType)).Cast<TemporalStatType>())
            data.Add($"{x.GetShortTerm()}, {x}");

        WriteCsv("StatTypes", data);
    }

    [MenuItem("Neon/Localization/Export Card Statuses")]
    public static void ExportCardStatuses()
    {
        var data = new List<string>();
        foreach (var action in GetAllInstances<CardActionsData>()
            .SelectMany(x => x.Actions)
            .Where(x => x.Type == CardBattleActionType.Battle && !string.IsNullOrWhiteSpace(x.BattleEffect.StatusDetailText))
            .Select(x => x.BattleEffect))
            data.Add($"{action.StatusDetailTerm.Split('/').Last()}^{ToSingleLineI2Format(action.StatusDetailText ?? "")}");
        WriteCsv("CardStatuses", data);
    }

    [MenuItem("Neon/Localization/Export Global Effects")]
    public static void ExportGlobalEffects()
    {
        var data = new List<string>();
        foreach (var effect in GetAllInstances<StaticGlobalEffect>())
        {
            data.Add($"{effect.ShortDescriptionTerm.Split('/').Last()}^{ToSingleLineI2Format(effect.Data.ShortDescription)}");
            data.Add($"{effect.FullDescriptionTerm.Split('/').Last()}^{ToSingleLineI2Format(effect.Data.FullDescription)}");
        }
        WriteCsv("GlobalEffects", data);
    }

    [MenuItem("Neon/Localization/Export Stages")]
    public static void ExportStages()
    {
        var data = new List<string>();
        foreach (var stage in GetAllInstances<HybridStageV5>())
            data.Add($"{stage.DisplayName.Split('/').Last()}^{ToSingleLineI2Format(stage.displayName)}");
        WriteCsv("stages", data);
    }
    
    [MenuItem("Neon/Localization/Export Corps")]
    public static void ExportCorps()
    {
        var data = new List<string>();
        foreach (var stage in GetAllInstances<StaticCorp>())
        {
            data.Add($"{stage.GetTerm().Split('/').Last()}^{ToSingleLineI2Format(stage.Name)}");
            data.Add($"{stage.ClinicNameTerm.Split('/').Last()}^{ToSingleLineI2Format(stage.clinicName)}");  
            data.Add($"{stage.GearShopData.ShopNameTerm.Split('/').Last()}^{ToSingleLineI2Format(stage.gearShopName)}");  
            data.Add($"{stage.ShortDescriptionTerm.Split('/').Last()}^{ToSingleLineI2Format(stage.shortDescription)}");  
            data.Add($"{stage.LongDescriptionTerm.Split('/').Last()}^{ToSingleLineI2Format(stage.longDescription)}");  
        }
        WriteCsv("corps", data);
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