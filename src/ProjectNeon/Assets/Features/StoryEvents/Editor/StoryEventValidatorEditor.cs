#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StoryEventValidatorEditor : EditorWindow
{
    [MenuItem("Neon/Validations")]
    static void Go()
    {
        GetWindow(typeof(StoryEventValidatorEditor)).Show();
    }
    
    void OnGUI()
    {
        if (GUILayout.Button("Validate All Story Event V2"))
        {
            var storyEvents = GetAllInstances<StoryEvent2>();
            var invalidStoryEventChoices = new List<string>();
            foreach (var storyEvent in storyEvents)
                foreach (var choice in storyEvent.Choices)
                    if (!choice.OddsTableIsValid)
                        invalidStoryEventChoices.Add($"Odds: [{choice.OddsTableTotal * 100:F0}%] - Story: [{storyEvent.DisplayName}] - Choice: [{choice.Choice}]");

            GetWindow<ListDisplayWindow>()
                .Initialized($"Invalid Story Event Choices Odds Tables", "", invalidStoryEventChoices.ToArray())
                .Show();
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
