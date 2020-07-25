using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleTestSetup))]
public class BattleTestSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {        
        var engine = (BattleTestSetup)target;
        DrawDefaultInspector();
        if(GUILayout.Button("Use Custom Party"))
            engine.UseCustomParty();
        if(GUILayout.Button("Use Custom Battlefield"))
            engine.UseCustomBattlefield();
    }
}
