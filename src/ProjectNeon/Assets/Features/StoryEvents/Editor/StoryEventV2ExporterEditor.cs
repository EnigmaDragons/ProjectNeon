#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class StoryEventV2ExporterEditor : EditorWindow
{
    [MenuItem("Neon/Export Story Events V2")]
    static void Go()
    {
        GetWindow(typeof(StoryEventV2ExporterEditor)).Show();
    }
    
    void OnGUI()
    { 
        if(!LocalizationSettings.InitializationOperation.IsDone) 
            LocalizationSettings.InitializationOperation.Task.GetAwaiter().GetResult();
        Localize.SetDb(LocalizationSettings.StringDatabase);
        if (GUILayout.Button("Export All"))
        {
            var storyEvents = GetAllInstances<StoryEvent2>();
            var storyEventStrings = new List<string>();
            foreach (var storyEvent in storyEvents)
                storyEventStrings.Add(storyEvent.ToString());

            var writer = new StreamWriter("StoryEventExports.txt", false);
            writer.Write(string.Join("\n", storyEventStrings));
            
            GUIUtility.ExitGUI();
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
    
    private void DrawUILine() => DrawUILine(Color.black);
    private void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
    }
}

#endif
