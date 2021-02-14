#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindEffectsEditor : EditorWindow
{
    [MenuItem("Tools/Neon/Find Effects")]
    static void SelectGameObjects()
    {
        GetWindow(typeof(FindEffectsEditor)).Show();
    }

    // By Effect Type
    private EffectType _effectType;
    private string[] GetAllCardEffectsWith(EffectType effectType) =>
        GetAllInstances<CardActionsData>()
            .Where(e => e.BattleEffects.Any(x => x.EffectType == effectType))
            .Select(e => $"Card {e.name}")
            .ToArray();
    private string[] GetAllEnemiesWith(EffectType effectType)=>
        GetAllInstances<Enemy>()
            .Where(e => e.Effects.Any(x => x.EffectType == effectType))
            .Select(e => $"Enemy {e.name}")
            .ToArray();
    private string[] GetAllEquipmentWith(EffectType effectType) => 
        GetAllInstances<StaticEquipment>()            
            .Where(e => e.AllEffects.Any(x => x.EffectType == effectType))
            .Select(e => $"Equipment {e.name}")
            .ToArray();
    
    void OnGUI()
    {
        _effectType = (EffectType)EditorGUILayout.EnumPopup("EffectType", _effectType);
        if (GUILayout.Button("Search By Effect Type"))
        {
            var effects = GetAllCardEffectsWith(_effectType)
                .Concat(GetAllEnemiesWith(_effectType))
                .Concat(GetAllEquipmentWith(_effectType));
            ShowItems($"Content Using Effect Type - {_effectType}", effects.ToArray());
            GUIUtility.ExitGUI();
        }
        DrawUILine();

//        if (GUILayout.Button("Show All Unused Effects"))
//        {
//            var zeroUsageResults = Enum.GetValues(typeof(EffectType)).Cast<EffectType>()
//                .Select(effectType => (effectType, GetAllCardEffectsWith(effectType)))
//                .Where(e => e.Item2.Length == 0)
//                .Where(e => e.effectType != EffectType.Nothing)
//                .Select(e => e.effectType.ToString())
//                .ToArray();
//            GetWindow<ListDisplayWindow>()
//                .Initialized("Unused Effect Types", zeroUsageResults)
//                .Show();
//            GUIUtility.ExitGUI();
//        }
//        DrawUILine();
    }
    
    private void ShowItems(string description, string[] items) 
        => GetWindow<ListDisplayWindow>()
            .Initialized(description, items)
            .Show();
    
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
