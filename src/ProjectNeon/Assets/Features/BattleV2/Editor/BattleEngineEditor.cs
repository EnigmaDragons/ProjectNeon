using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleEngine))]
public class BattleEngineEditor : Editor
{
    public override void OnInspectorGUI()
    {        
        var engine = (BattleEngine)target;
        if(GUILayout.Button("Setup Battle"))
            engine.Setup();
        DrawDefaultInspector();
    }
}