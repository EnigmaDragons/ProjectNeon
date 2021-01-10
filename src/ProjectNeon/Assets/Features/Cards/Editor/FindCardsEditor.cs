#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindCardsEditor : EditorWindow
{
    [MenuItem("Tools/Neon/Find Cards")]
    static void SelectGameObjects()
    {
        GetWindow(typeof(FindCardsEditor)).Show();
    }

    // By Effect Type
    private EffectType _effectType;
    private string[] GetAllEffectsWithType(EffectType effectType) =>
        GetAllInstances<CardActionsData>()
            .Where(e => e.Actions.Any(x => x.Type == CardBattleActionType.Battle && x.BattleEffect.EffectType == effectType))
            .Select(e => e.name)
            .ToArray();
    
    // By Avoidance Type
    private AvoidanceType _avoidanceType;
    private string[] GetAllEffectsWithType(AvoidanceType avoidanceType) =>
        GetAllInstances<CardType>()
            .Where(e => e.ActionSequences.Any(x => x.AvoidanceType == avoidanceType))
            .Select(e => e.name)
            .ToArray();
    
    // By Description Search String
    private string _searchString;
    private string[] GetAllCardsWithDescription(string s) =>
        GetAllInstances<CardType>()
            .Where(c => c.Description.Contains(s))
            .Select(e => e.name)
            .ToArray();
    
    void OnGUI()
    {
        _effectType = (EffectType)EditorGUILayout.EnumPopup("EffectType", _effectType);
        if (GUILayout.Button("Search By Effect Type")) 
        {
            var effects = GetAllEffectsWithType(_effectType);
            GetWindow<ListDisplayWindow>()
                .Initialized($"{_effectType} - {effects.Length} uses", effects)
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();

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
        DrawUILine();
        
        _avoidanceType = (AvoidanceType)EditorGUILayout.EnumPopup("AvoidanceType", _avoidanceType);
        if (GUILayout.Button("Search By Avoidance Type")) 
        {
            var cards = GetAllEffectsWithType(_avoidanceType);
            GetWindow<ListDisplayWindow>()
                .Initialized($"{_avoidanceType} - {cards.Length} uses", cards)
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();

        if (GUILayout.Button("Show All X Cost Cards"))
        {
            var xCostResults = GetAllInstances<CardType>()
                .Where(c => c.Cost.PlusXCost)
                .Select(e => e.name)
                .ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized("X Cost Cards", xCostResults)
                .Show();
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        _searchString = GUILayout.TextField(_searchString);
        if (GUILayout.Button("Find Term in Card Description"))
        {
            var cards = GetAllCardsWithDescription(_searchString);
            GetWindow<ListDisplayWindow>()
                .Initialized($"Description Containing {_searchString}", cards)
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
