#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindAllCardsByAvoidanceTypeEditor : EditorWindow
{
    private AvoidanceType _avoidanceType;
    
    [MenuItem("Tools/Neon/Find All Cards By Avoidance Type")]
    static void SelectGameObjects()
    {
        GetWindow(typeof(FindAllCardsByAvoidanceTypeEditor)).Show();
    }

    private string[] GetAllEffectsWithType(AvoidanceType avoidanceType) =>
        GetAllInstances<CardType>()
            .Where(e => e.ActionSequences.Any(x => x.AvoidanceType == avoidanceType))
            .Select(e => e.name)
            .ToArray();

    void OnGUI()
    {
        _avoidanceType = (AvoidanceType)EditorGUILayout.EnumPopup("AvoidanceType", _avoidanceType);
        
        if (GUILayout.Button("Search")) 
        {
            var cards = GetAllEffectsWithType(_avoidanceType);
            GetWindow<ListDisplayWindow>()
                .Initialized($"{_avoidanceType} - {cards.Length} uses", cards)
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
}

#endif
