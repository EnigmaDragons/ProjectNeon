#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public sealed class ListDisplayWindow : EditorWindow
{
    public string[] Items;
    public string Title;

    public ListDisplayWindow Initialized(string title, string[] items)
    {
        Title = title;
        Items = items;
        return this;
    }
    
    public void OnGUI()
    {
        EditorGUILayout.LabelField(Title);
        DrawUILine(Color.black);
        
        Items.ForEach(i => EditorGUILayout.LabelField("Effect: ", i));
        DrawUILine(Color.black);
        if (GUILayout.Button("Dismiss")) 
            Close();
        
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
