#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindCardsEditor : EditorWindow
{
    [MenuItem("Neon/Find Cards")]
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
    
    // By Description Search String
    private string _searchString;
    private string[] GetAllCardsWithDescription(string s) =>
        GetAllInstances<CardType>()
            .Where(c => c.Description.ContainsAnyCase(s))
            .Select(e => e.name)
            .ToArray();
    
    // By Card Tags
    private CardTag _cardTag;
    
    // By Card Type Description
    private string _cardTypeDescription;

    void OnGUI()
    {
        _effectType = (EffectType)EditorGUILayout.EnumPopup("EffectType", _effectType);
        if (GUILayout.Button("Search By Effect Type")) 
        {
            var effects = GetAllInstances<CardActionsData>()
                .Where(e => e.Actions.Any(x => x.Type == CardBattleActionType.Battle && x.BattleEffect.EffectType == _effectType))
                .Select(e => e.name)
                .ToArray();
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

        if (GUILayout.Button("Show All X Cost Cards"))
        {
            var xCostResults = GetAllInstances<CardType>()
                .Where(c => c.Cost.PlusXCost)
                .Select(e => e.name)
                .ToArray();
            ShowCards("X Cost Cards", xCostResults);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        _searchString = GUILayout.TextField(_searchString);
        if (GUILayout.Button("Find Term in Card Description"))
        {
            var cards = GetAllCardsWithDescription(_searchString);
            ShowCards($"Description Containing {_searchString}", cards);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        if (GUILayout.Button("Find Cards Without At Target Animations"))
        {
            var cards = GetAllInstances<CardType>()
                .Where(c => c.AllCardEffectSteps.None(t => t.Type == CardBattleActionType.AnimateAtTarget))
                .Select(e => e.name)
                .ToArray();
            ShowCards("Cards Without At Target Animations", cards);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        if (GUILayout.Button("Find Cards With Blank Animation Name"))
        {
            var cards = GetAllInstances<CardType>()
                .Where(c => c.AllCardEffectSteps.Any(t => t.Type == CardBattleActionType.AnimateAtTarget 
                                                          && string.IsNullOrWhiteSpace(t.AtTargetAnimation.Animation)))
                .Select(e => e.name)
                .ToArray();
            ShowCards("Cards With Blank Animation Name", cards);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        _cardTag = (CardTag)EditorGUILayout.EnumPopup("CardTag", _cardTag);
        if (GUILayout.Button("Find Cards With Card Tag"))
        {
            var cards = GetAllInstances<CardType>()
                .Where(c => c.Tags.Any(t => t == _cardTag))
                .Select(e => e.name)
                .ToArray();
            ShowCards("Cards With Card Tag", cards);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        _cardTypeDescription = GUILayout.TextField(_cardTypeDescription);
        if (GUILayout.Button("Find By Card Type Description"))
        {
            var cards = GetAllInstances<CardType>()
                .Where(c => c.TypeDescription.Equals(_cardTypeDescription, StringComparison.InvariantCultureIgnoreCase))
                .Select(e => e.name)
                .ToArray();
            ShowCards($"Card Type Description Is {_searchString}", cards);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
    }
    
    private void ShowCards(string description, string[] cards) 
        => GetWindow<ListDisplayWindow>()
            .Initialized(description, cards)
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
