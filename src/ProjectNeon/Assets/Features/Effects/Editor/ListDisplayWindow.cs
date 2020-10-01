using UnityEditor;
using UnityEngine;

public sealed class ListDisplayWindow : EditorWindow
{
    public string[] Items { get; set; }

    public void OnGUI()
    {
        Items.ForEach(i => EditorGUILayout.LabelField("Effect: ", i));
        
        if (GUILayout.Button("Dismiss")) 
            Close();
        
        GUIUtility.ExitGUI();
    }
}

