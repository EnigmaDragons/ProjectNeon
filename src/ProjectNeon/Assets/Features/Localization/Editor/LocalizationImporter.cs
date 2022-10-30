#if UNITY_EDITOR

using System;
using System.Linq;
using System.Xml.Schema;
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
        ImportCutscenes(true);
        ImportStoryEvents(true);
        ImportLoadingScreens(true);
        ImportEnemies(true);
    }

    [MenuItem("Neon/Localization/Import Tutorial Slides")]
    private static void ImportTutorialSlides()
        => ImportTutorialSlides(false);
    private static void ImportTutorialSlides(bool hasInit)
        => ImportItems<TutorialSlide>(x => x.Term, (s, text) => s.text = text, hasInit);

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
    {
        if (!hasInit)
            LocalizationManager.UpdateSources();
        foreach (var item in GetAllInstances<T>())
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