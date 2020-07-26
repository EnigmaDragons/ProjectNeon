using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleTestSetup))]
public class BattleTestSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {        
        var engine = (BattleTestSetup)target;
        DrawDefaultInspector();
        
        DrawUILine(Color.black);
        if(GUILayout.Button("Use Everything And Start Battle"))
            engine.UseEverythingAndStartBattle();
        DrawUILine(Color.black);
        
        if(GUILayout.Button("1. Use Custom Battlefield"))
            engine.UseCustomBattlefield();
        if(GUILayout.Button("2. Use Custom Party"))
            engine.UseCustomParty();
        if(GUILayout.Button("3a. Use Custom Enemies"))
            engine.UseFixedEncounter();
        if(GUILayout.Button("3b. Use Custom Encounter Set"))
            engine.UseCustomEncounterSet();
        if(GUILayout.Button("4. Setup Battle"))
            engine.SetupBattle();
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
