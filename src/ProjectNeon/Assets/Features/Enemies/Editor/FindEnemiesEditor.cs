#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindEnemiesEditor : EditorWindow
{
    [MenuItem("Neon/Find Enemies")]
    static void Open() => GetWindow(typeof(FindEnemiesEditor)).Show();

    private string _corpName;
    
    void OnGUI()
    {
        if (GUILayout.Button("Without Corps")) 
        {
            var items = GetAllInstances<Enemy>()
                .Where(e => e.Corp == null)
                .Select(e => e.name)
                .ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized($"WithoutCorps - {items.Length} Enemies", items)
                .Show();
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("By AI"))
        {
            var items = GetAllInstances<Enemy>()
                .Where(e => e.IsCurrentlyWorking)
                .Select(e => $"{e.ForStage(0).AI.name} - {e.name}")
                .OrderBy(e => e.StartsWith("GeneralAI") ? 99 : 0)
                .ThenBy(e => e)
                .ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized($"Enemies by AI", "Enemy:", items)
                .Show();
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("AI Preferences"))
        {
            var items = GetAllWorkingEnemies()
                .Select(e =>
                {
                    var p = e.ForStage(0).AIPreferences;
                    return $"{AiPrefName(p)} - {e.name} - {p.GetCustomizationDescription()}";
                })
                .OrderBy(e => e)
                .ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized($"Enemies by AI Preferences", "Enemy:", items)
                .Show();
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Enemy Descriptions"))
        {
            var items = GetAllWorkingEnemies()
                .Select(e => $"{e.name} - {e.Description}")
                .OrderBy(e => e)
                .ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized($"Enemy Descriptions", "Enemy:", items)
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();

        _corpName = GUILayout.TextField(_corpName);
        if (GUILayout.Button("By Corps")) 
        {
            var items = GetAllInstances<Enemy>()
                .Where(e => e.Corp != null && e.Corp.Name.Equals(_corpName, StringComparison.InvariantCultureIgnoreCase))
                .Select(e => e.name)
                .ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized($"{_corpName} - {items.Length} Enemies", items)
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        if (GUILayout.Button("Reset Card Order Factor"))
        {
            var items = GetAllInstances<Enemy>();
            items.ForEach(e =>
            {
                e.aiPreferences.CardOrderPreferenceFactor = 0;
                EditorUtility.SetDirty(e);
            });
        }
        if (GUILayout.Button("Simplify Specialist Attack Unpreferred"))
        {
            var items = GetAllInstances<Enemy>().Where(e => e.IsCurrentlyWorking && e.BattleRole == BattleRole.Specialist);
            items.ForEach(e =>
            {
                if (e.aiPreferences.UnpreferredCardTags.Any(t => t == CardTag.Attack))
                    e.aiPreferences.UnpreferredCardTags = e.aiPreferences.UnpreferredCardTags.Except(CardTag.Attack).ToArray();
                EditorUtility.SetDirty(e);
            });
        }
        DrawUILine();
    }

    private IEnumerable<Enemy> GetAllWorkingEnemies() => GetAllInstances<Enemy>().Where(e => e.IsCurrentlyWorking);

    private static string AiPrefName(AiPreferences p) => p.IsDefault ? "Default" : "Custom";
    
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
