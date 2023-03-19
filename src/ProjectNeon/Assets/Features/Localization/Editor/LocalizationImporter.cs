#if UNITY_EDITOR

using System;
using System.Linq;
using I2.Loc;
using UnityEditor;
using UnityEngine;

public class LocalizationImporter
{
    [MenuItem("Neon/Localization/Import All")]
    private static void ImportAll()
    {
        LocalizationManager.UpdateSources();
        ImportTutorialSlides(true);
        ImportTutorialSlideshows(true);
        ImportCutscenes(true);
        ImportStoryEvents(true);
        ImportLoadingScreens(true);
        ImportEnemies(true);
        ImportHeroes(true);
        ImportBlessings(true);
        ImportDifficulties(true);
        ImportAdventures(true);
        ImportEquipments(true);
        ImportCardStatuses(true);
        ImportGlobalEffects(true);
        ImportCorps(true);
        ImportKeyTermCollections(true);
        ImportStages(true);
    }
    
    [MenuItem("Neon/Localization/Import Equipments")]
    private static void ImportEquipments()
        => ImportEquipments(false);
    private static void ImportEquipments(bool hasInit)
        => ImportItems<StaticEquipment>(x => x.LocalizationDescriptionTerm(), (s, text) => s.description = text, hasInit);

    [MenuItem("Neon/Localization/Import Tutorial Slides")]
    private static void ImportTutorialSlides()
        => ImportTutorialSlides(false);
    private static void ImportTutorialSlides(bool hasInit)
        => ImportItems<TutorialSlide>(x => x.Term, (s, text) => s.text = text, hasInit);

    [MenuItem("Neon/Localization/Import Tutorial Slideshows")]
    private static void ImportTutorialSlideshows()
        => ImportTutorialSlideshows(false);
    private static void ImportTutorialSlideshows(bool hasInit)
        => ImportItems<TutorialSlideshow>(x => x.DisplayNameTerm, (s, text) => s.displayName = text, hasInit);
    
    [MenuItem("Neon/Localization/Import Cutscenes")]
    private static void ImportCutscenes()
        => ImportCutscenes(false);
    private static void ImportCutscenes(bool hasInit)
        => ImportSubItems<Cutscene, CutsceneSegmentData>(x => x.Segments.Where(x => x.SegmentType == CutsceneSegmentType.DialogueLine || x.SegmentType == CutsceneSegmentType.NarratorLine || x.SegmentType == CutsceneSegmentType.PlayerLine).ToArray(), x => x.Term, (x, text) => x.Text = text, hasInit);

    [MenuItem("Neon/Localization/Import Story Events")]
    private static void ImportStoryEvents()
        => ImportStoryEvents(false);
    private static void ImportStoryEvents(bool hasInit)
    {
        ImportItems<StoryEvent2>(x => x.Term, (x, text) => x.storyText = text, hasInit);
        ImportSubItems<StoryEvent2, StoryEventChoice2>(x => x.Choices, choice => choice.Term, (choice, text) => choice.Choice = text, hasInit);
    }

    [MenuItem("Neon/Localization/Import Loading Screens")]
    private static void ImportLoadingScreens()
        => ImportLoadingScreens(false);
    private static void ImportLoadingScreens(bool hasInit)
        => ImportItems<CorpLoadingScreen>(x => x.Term, (x, text) => x.locationTitle = text, hasInit);

    [MenuItem("Neon/Localization/Import Enemies")]
    private static void ImportEnemies()
        => ImportEnemies(false);
    private static void ImportEnemies(bool hasInit)
    {
        ImportItems<Enemy>(x => x.EnemyNameTerm, (enemy, text) => enemy.enemyName = text, hasInit);
        ImportItems<Enemy>(x => x.DescriptionTerm, (enemy, text) => enemy.description = text, hasInit);
    }

    [MenuItem("Neon/Localization/Import Heroes")]
    private static void ImportHeroes()
        => ImportHeroes(false);
    private static void ImportHeroes(bool hasInit)
    {
        ImportItems<BaseHero>(x => x.ClassTerm(), (hero, text) => hero.className = text, hasInit);
        ImportItems<BaseHero>(x => x.DescriptionTerm(), (hero, text) => hero.flavorDetails.HeroDescription = text, hasInit);
        ImportItems<BaseHero>(x => x.BackStoryTerm(), (hero, text) => hero.flavorDetails.BackStory = text, hasInit);
    }

    [MenuItem("Neon/Localization/Import Blessings")]
    private static void ImportBlessings()
        => ImportBlessings(false);
    private static void ImportBlessings(bool hasInit)
    {
        ImportSubItems<CorpClinicProvider, BlessingData>(p => p.blessingsV4, b => b.NameTerm, (b, text) => b.Name = text, hasInit);
        ImportSubItems<CorpClinicProvider, BlessingData>(p => p.blessingsV4, b => b.DescriptionTerm, (b, text) => b.Description = text, hasInit); 
    }
    
    [MenuItem("Neon/Localization/Import Difficulties")]
    private static void ImportDifficulties()
        => ImportDifficulties(false);
    private static void ImportDifficulties(bool hasInit)
    {
        ImportItems<Difficulty>(x => x.NameTerm, (d, text) => d.difficultyName = text, hasInit);
        ImportItems<Difficulty>(x => x.DescriptionTerm, (d, text) => d.description = text, hasInit);
        ImportItems<Difficulty>(x => x.ChangesTerm, (d, text) => d.changes = text, hasInit);
    }
    
    [MenuItem("Neon/Localization/Import Adventures")]
    private static void ImportAdventures()
        => ImportAdventures(false);
    private static void ImportAdventures(bool hasInit)
    {
        ImportItems<Adventure>(x => x.TitleTerm, (a, text) => a.adventureTitle = text, hasInit);
        ImportItems<Adventure>(x => x.RawMapTitleTerm, (a, text) => a.mapAdventureTitle = text, hasInit);
        ImportItems<Adventure>(x => x.AllowedHeroesDescriptionTerm, (a, text) => a.allowedHeroesDescription = text, hasInit);
        ImportItems<Adventure>(x => x.StoryTerm, (a, text) => a.story = text, hasInit);
        ImportItems<Adventure>(x => x.LockConditionExplanationTerm, (a, text) => a.lockConditionExplanation = text, hasInit);
        ImportItems<Adventure>(x => x.VictoryConclusionTerm, (a, text) => a.victoryConclusion = text, hasInit);
        ImportItems<Adventure>(x => x.DefeatConclusionTerm, (a, text) => a.defeatConclusion = text, hasInit);
    }
    
    [MenuItem("Neon/Localization/Import Card Statuses")]
    private static void ImportCardStatuses()
        => ImportAdventures(false);
    private static void ImportCardStatuses(bool hasInit)
    {
        ImportSubItems<CardActionsData, EffectData>( 
            c => c.Actions
                .Where(a => a.Type == CardBattleActionType.Battle)
                .Select(a => a.BattleEffect)
                .ToArray(), 
            e => e.StatusDetailTerm, 
            (e, text) => e.StatusDetailText = text, 
            hasInit);
    }

    [MenuItem("Neon/Localization/Import Global Effects")]
    private static void ImportGlobalEffects()
        => ImportGlobalEffects(false);
    private static void ImportGlobalEffects(bool hasInit)
    {
        ImportItems<StaticGlobalEffect>(e => e.ShortDescriptionTerm, (e, text) => e.Data.ShortDescription = text, hasInit);
        ImportItems<StaticGlobalEffect>(e => e.FullDescriptionTerm, (e, text) => e.Data.FullDescription = text, hasInit);
    }

    [MenuItem("Neon/Localization/Import Corps")]
    private static void ImportCorps()
        => ImportCorps(false);
    private static void ImportCorps(bool hasInit)
    {
        ImportItems<StaticCorp>(e => e.GearShopData.ShopNameTerm, (e, text) => e.gearShopName = text, hasInit);
        ImportItems<StaticCorp>(e => e.ClinicNameTerm, (e, text) => e.clinicName = text, hasInit);
        ImportItems<StaticCorp>(e => e.ShortDescriptionTerm, (e, text) => e.shortDescription = text, hasInit);
        ImportItems<StaticCorp>(e => e.LongDescriptionTerm, (e, text) => e.longDescription = text, hasInit);
    }
    
    [MenuItem("Neon/Localization/Import Key Term Collections")]
    private static void ImportKeyTermCollections()
        => ImportKeyTermCollections(false);
    private static void ImportKeyTermCollections(bool hasInit)
    {
        ImportSubItems<StringKeyTermCollection, StringKeyTermPair>(c => c.All.ToArray(), p => p.Term, (p, text) => p.Value = text, hasInit);
    }
    
    [MenuItem("Neon/Localization/Import Stages")]
    private static void ImportStages()
        => ImportStages(false);
    private static void ImportStages(bool hasInit)
    {
        ImportItems<HybridStageV5>(p => p.DisplayName, (p, text) => p.displayName = text, hasInit);
    }
    
    private static void ImportItems<T>(Func<T, string> getTerm, Action<T, string> setText, bool hasInit) where T : ScriptableObject
    {
        if (!hasInit)
            LocalizationManager.UpdateSources();
        foreach (var item in GetAllInstances<T>())
        {
            var term = getTerm(item);
            var text = term.ToEnglish();
            if (string.IsNullOrWhiteSpace(text) || text == term)
                Debug.LogError($"Could not translate {term}");
            else
            {
                setText(item, text);
                EditorUtility.SetDirty(item);
            }
        }
    }

    private static void ImportSubItems<T, T2>(Func<T, T2[]> getSubItems, Func<T2, string> getTerm, Action<T2, string> setText, bool hasInit) where T : ScriptableObject
        => ImportSubItems(_ => true, getSubItems, getTerm, setText, hasInit);
        
    private static void ImportSubItems<T, T2>(Func<T, bool> conditional, Func<T, T2[]> getSubItems, Func<T2, string> getTerm, Action<T2, string> setText, bool hasInit) where T : ScriptableObject
    {
        if (!hasInit)
            LocalizationManager.UpdateSources();
        foreach (var item in GetAllInstances<T>().Where(conditional))
        {
            var hasSetDirty = false;
            foreach (var subItem in getSubItems(item))
            {
                var term = getTerm(subItem);
                var text = term.ToEnglish();
                if (string.IsNullOrWhiteSpace(text) || text == term)
                    Debug.LogError($"Could not translate {term}");
                else
                {
                    setText(subItem, text);
                    if (!hasSetDirty)
                    {
                        EditorUtility.SetDirty(item);
                        hasSetDirty = true;
                    }
                }
            }
        }
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