using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindAllEffectsOfTypeEditor : EditorWindow
{
    private EffectType _effectType;
    
    [MenuItem("Tools/Neon/Find All Effects Of Type")]
    static void SelectGameObjects()
    {
        GetWindow(typeof(FindAllEffectsOfTypeEditor)).Show();
    }

    private string[] GetAllEffectsWithType(EffectType effectType) =>
        GetAllInstances<CardActionsData>()
            .Where(e => e.Actions.Any(x => x.Type == CardBattleActionType.Battle && x.BattleEffect.EffectType == effectType))
            .Select(e => e.name)
            .ToArray();

    void OnGUI()
    {
        _effectType = (EffectType)EditorGUILayout.EnumPopup("EffectType", _effectType);
        
        if (GUILayout.Button("Search")) 
        {
            var effects = GetAllEffectsWithType(_effectType);
            GetWindow<ListDisplayWindow>()
                .Initialized($"{_effectType} - {effects.Length} uses", effects)
                .Show();
            GUIUtility.ExitGUI();
        }

        if (GUILayout.Button("Show All Unused Effects"))
        {
            var zeroUsageResults = Enum.GetValues(typeof(EffectType)).Cast<EffectType>()
                .Select(effectType => (effectType, GetAllEffectsWithType(effectType)))
                .Where(e => e.Item2.Length == 0)
                .Where(e => e.effectType != EffectType.Nothing)
                .Select(e => e.effectType.ToString())
                .ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized("Unused Effect Types", zeroUsageResults)
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
