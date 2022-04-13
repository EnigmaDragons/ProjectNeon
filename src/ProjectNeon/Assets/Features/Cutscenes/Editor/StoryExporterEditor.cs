#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class StoryExporterEditor : EditorWindow
{
    [MenuItem("Neon/Story Exporter")]
    static void Open() => GetWindow(typeof(StoryExporterEditor)).Show();

    private int _adventureId;
    private string _exportFilePath;
    private static string _divider = "---------------------------------------------------------------------";
    
    void OnGUI()
    {
        GUILayout.Label("Adventure ID");
        if (int.TryParse(GUILayout.TextField(_adventureId.ToString()), out var id))
            _adventureId = id;

        GUILayout.Label("Export File Path");
        _exportFilePath = GUILayout.TextField(_exportFilePath);
        
        if (GUILayout.Button("Preview Story"))
        {
            GenerateStory(_adventureId).IfPresent(storyLines =>
                GetWindow<ListDisplayWindow>()
                    .Initialized($"Story", "", storyLines.ToArray())
                    .Show());
        }

        if (GUILayout.Button("Export Story"))
        {
            GenerateStory(_adventureId).IfPresent(storyLines =>
            {
                File.WriteAllText(_exportFilePath, string.Join("\n", storyLines));
                Log.Info("Exported Story to " + _exportFilePath);
            });
        }
    }

    private static Maybe<List<string>> GenerateStory(int adventureId)
    {
        var matchingAdventure = GetAllInstances<Adventure>().FirstOrMaybe(a => a.Id == adventureId);
        return matchingAdventure.Select(a =>
        {
            var storyLines = new List<string>();
            storyLines.Add("Story Title: " + a.Title);
            storyLines.Add(_divider);
            
            storyLines.Add("Story: Intro");
            storyLines.Add("  " + a.Story);
            storyLines.Add(_divider);

            var storySegments = a.StagesV5.SelectMany(s => s.MaybeStorySegments);
            var sceneNumber = 0;
            storySegments.ForEach(s =>
            {
                if (s == null)
                    return;

                var c = s as CutsceneStageSegment;
                if (c == null)
                    return;

                ++sceneNumber;
                var cutscene = c.Cutscene;
                storyLines.Add($"Story Scene: {sceneNumber} - Setting: {cutscene.Setting.GetDisplayName()}");
                var lines = cutscene.Segments;
                var lastCondition = "";
                foreach (var l in lines)
                {
                    if (l.SegmentType == CutsceneSegmentType.Wait)
                        continue;

                    var condition = l.GetRequiredConditionsDescription();
                    condition.ExecuteIfPresentOrElse(d =>
                    {
                        if (!d.Equals(lastCondition, StringComparison.InvariantCultureIgnoreCase))
                            storyLines.Add("  " + d);
                        storyLines.Add("      " + l.GetExportDescription());
                        lastCondition = d;
                    }, () =>
                    {
                        storyLines.Add("  " + l.GetExportDescription());
                        lastCondition = "";
                    });
                }

                storyLines.Add(_divider);
            });
            return storyLines;
        }, () => Maybe<List<string>>.Missing());
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