#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class ListDisplayWindow : EditorWindow
{
    public string[] Items;
    public string[] Labels;
    public string Title;

    Vector2 scrollPosition = Vector2.zero;

    
    public ListDisplayWindow Initialized(string title, string[] items)
    {
        Title = title;
        Items = items;
        Labels = items.Select(_ => "Effect: ").ToArray();
        return this;
    }
    
    public ListDisplayWindow Initialized(string title, string itemLabel, string[] items)
    {
        Title = title;
        Items = items;
        Labels = items.Select(_ => itemLabel).ToArray();
        return this;
    }
    
    public ListDisplayWindow Initialized(string title, string[] labels, string[] items)
    {
        Title = title;
        Items = items;
        Labels = labels;
        return this;
    }
    
    public void OnGUI()
    {
        EditorGUILayout.LabelField(Title);
        DrawUILine(Color.black);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 16));
        Items.ForEachIndex((item, idx) => EditorGUILayout.LabelField(Labels[idx], item, GUILayout.Width(Screen.width - 8)));
        DrawUILine(Color.black);
        if (GUILayout.Button("Dismiss")) 
            Close();
        EditorGUILayout.EndScrollView();
        
        GUIUtility.ExitGUI();
    }
    
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
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
