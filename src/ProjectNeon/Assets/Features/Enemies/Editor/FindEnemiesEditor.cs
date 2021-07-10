#if UNITY_EDITOR
using System;
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
