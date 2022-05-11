#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindEffectsEditor : EditorWindow
{
    [MenuItem("Neon/Find Effects")]
    static void SelectGameObjects()
    {
        GetWindow(typeof(FindEffectsEditor)).Show();
    }

    // By Effect Type
    private EffectType _effectType;
    private string _effectScope;
    private string _formulaSearchString;
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
    
    private string[] GetAllContentWithEffectType(EffectType e)
        =>  GetAllCardEffectsWith(e)
                .Concat(GetAllEnemiesWith(e))
                .Concat(GetAllEquipmentWith(e)).ToArray();
    
    private string[] GetAllContentWithEffectScope(string effectScope)
        => GetAllInstances<CardActionsData>()
            .Where(e => e.BattleEffects.Any(x => x.EffectScope != null && x.EffectScope.Value.Equals(effectScope)))
            .Select(e => $"Card {e.name}")
        .Concat(GetAllInstances<Enemy>()
            .Where(e => e.Effects.Any(x => x.EffectScope != null && x.EffectScope.Value.Equals(effectScope)))
            .Select(e => $"Enemy {e.name}"))
        .Concat(GetAllInstances<StaticEquipment>()            
            .Where(e => e.AllEffects.Any(x => x.EffectScope != null && x.EffectScope.Value.Equals(effectScope)))
            .Select(e => $"Equipment {e.name}"))
        .ToArray();
    
    private string[] GetAllContentWithEffect(Func<EffectData, bool> condition)
        => GetAllInstances<CardActionsData>()
            .Where(e => e.BattleEffects.Any(condition))
            .Select(e => $"Card {e.name}")
            .Concat(GetAllInstances<Enemy>()
                .Where(e => e.Effects.Any(condition))
                .Select(e => $"Enemy {e.name}"))
            .Concat(GetAllInstances<StaticEquipment>()            
                .Where(e => e.AllEffects.Any(condition))
                .Select(e => $"Equipment {e.name}"))
            .ToArray();
    
    private string[] GetAllEffectsWithFormula(string searchString)
        => GetAllInstances<CardActionsData>()
            .Where(e => e.BattleEffects.Any(x => x.Formula != null && x.Formula.ContainsAnyCase(searchString)))
            .Select(e => $"Card {e.name}")
            .Concat(GetAllInstances<Enemy>()
                .Where(e => e.Effects.Any(x => x.Formula != null && x.Formula.ContainsAnyCase(searchString)))
                .Select(e => $"Enemy {e.name}"))
            .Concat(GetAllInstances<StaticEquipment>()            
                .Where(e => e.AllEffects.Any(x => x.Formula != null && x.Formula.ContainsAnyCase(searchString)))
                .Select(e => $"Equipment {e.name}"))
            .ToArray();
    
    
    private string[] GetAllContentWithLostEffectType() =>  
                GetAllInstances<CardActionsData>()
                .Where(e => e.BattleEffects.Any(x => (int)x.EffectType == -1))
                .Select(e => $"Card {e.name}")
            .Concat(
                GetAllInstances<Enemy>()
                .Where(e => e.Effects.Any(x => (int)x.EffectType == -1))
                .Select(e => $"Enemy {e.name}")
            .Concat(
                GetAllInstances<StaticEquipment>()            
                .Where(e => e.AllEffects.Any(x => (int)x.EffectType == -1))
                .Select(e => $"Equipment {e.name}")))
            .ToArray();

    private string[] GetAllCardEffectsWith(ReactionConditionType t) =>
        GetAllInstances<CardActionsData>()
            .Where(e => e.BattleEffects.Any(x => x.ReactionConditionType == t))
            .Select(e => $"Card {e.name}")
            .ToArray();
    private string[] GetAllEnemiesWith(ReactionConditionType t)=>
        GetAllInstances<Enemy>()
            .Where(e => e.Effects.Any(x => x.ReactionConditionType == t))
            .Select(e => $"Enemy {e.name}")
            .ToArray();
    private string[] GetAllEquipmentWith(ReactionConditionType t) => 
        GetAllInstances<StaticEquipment>()            
            .Where(e => e.AllEffects.Any(x => x.ReactionConditionType == t))
            .Select(e => $"Equipment {e.name}")
            .ToArray();
    
    private string[] GetAllContentWithReactionConditionType(ReactionConditionType e)
        =>  GetAllCardEffectsWith(e)
            .Concat(GetAllEnemiesWith(e))
            .Concat(GetAllEquipmentWith(e)).ToArray();
    
    private ReactionConditionType _reactionConditionType;
    
    void OnGUI()
    {
        _effectType = (EffectType)EditorGUILayout.EnumPopup("EffectType", _effectType);
        if (GUILayout.Button("Search By Effect Type"))
        {
            ShowItems($"Content Using Effect Type - {_effectType}", GetAllContentWithEffectType(_effectType));
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        _reactionConditionType = (ReactionConditionType)EditorGUILayout.EnumPopup("ReactionConditionType", _reactionConditionType);
        if (GUILayout.Button("Search By Reaction Condition Type"))
        {
            ShowItems($"Content Using Effect Type - {_effectType}", GetAllContentWithReactionConditionType(_reactionConditionType));
            GUIUtility.ExitGUI();
        }
        DrawUILine();

        _effectScope = GUILayout.TextField(_effectScope);
        if (GUILayout.Button("Search By Effect Scope"))
        {
            ShowItems($"Content Using Effect Scope - {_effectScope}", GetAllContentWithEffectScope(_effectScope));
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        _formulaSearchString = GUILayout.TextField(_formulaSearchString);
        if (GUILayout.Button("Find Formulas With Text"))
        {
            ShowItems($"Content With Formula Text - '{_formulaSearchString}'", GetAllEffectsWithFormula(_formulaSearchString));
            GUIUtility.ExitGUI();
        }
        DrawUILine();

        if (GUILayout.Button("Show All Unused Effects"))
        {
            var zeroUsageResults = Enum.GetValues(typeof(EffectType)).Cast<EffectType>()
                .Where(e => e != EffectType.Nothing)
                .Select(effectType => (effectType, GetAllContentWithEffectType(effectType)))
                .Where(e => e.Item2.Length == 0)
                .Select(e => e.effectType.ToString())
                .ToArray();
            ShowItems("Unused Effect Types", zeroUsageResults);
            GUIUtility.ExitGUI();
        }
        DrawUILine();

        if (GUILayout.Button("Show All Reaction Timing Windows"))
        {
            var effectType = EffectType.ReactWithEffect;
            var enemies = GetAllInstances<Enemy>()
                .SelectMany(e => e.Effects.Where(x => x.EffectType == effectType).Select(x => $"{x.FinalReactionTimingWindow} - {e.EnemyName}"));
            var equips = GetAllInstances<StaticEquipment>()
                .SelectMany(e => e.AllEffects.Where(x => x.EffectType == effectType).Select(x => $"{x.FinalReactionTimingWindow} - {e.Name}"));
            var cards = GetAllInstances<CardActionsData>()
                .SelectMany(e => e.BattleEffects.Where(x => x.EffectType == effectType).Select(x => $"{x.FinalReactionTimingWindow} - {e.name}"));
            var finalCollections = enemies.Concat(equips).Concat(cards).ToArray();
            ShowItems("Reactions", finalCollections);
            GUIUtility.ExitGUI();
        }
        DrawUILine();
        
        if (GUILayout.Button("Show Broken Content (Experimental)"))
        {
            ShowItems("Broken Effect Content", GetAllContentWithLostEffectType());
            GUIUtility.ExitGUI();
        }
        DrawUILine();
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
