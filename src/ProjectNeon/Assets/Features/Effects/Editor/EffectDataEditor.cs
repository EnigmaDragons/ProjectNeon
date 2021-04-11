#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EffectData))]
public class EffectDataEditor : PropertyDrawer
{
    private List<string> _globalProperties = new List<string> { "TurnDelay" };
    
    private DictionaryWithDefault<EffectType, string[]> _relevantProperties = new DictionaryWithDefault<EffectType, string[]>(new string[] { "BaseAmount", "FloatAmount", "NumberOfTurns", "HitsRandomTargetMember" })
    {
        {EffectType.Nothing, new string[0]},
        {EffectType.ReactWithEffect, new []{ "FloatAmount", "NumberOfTurns", "EffectScope", "StatusTag", "StatusDetailText", "ReactionConditionType", "ReactionEffectScope", "ReactionEffect"}},
        {EffectType.ReactWithCard, new []{ "FloatAmount", "NumberOfTurns", "EffectScope", "StatusTag", "StatusDetailText", "ReactionConditionType", "ReactionEffectScope", "ReactionSequence"}},
        {EffectType.AdjustResourceFlat, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.ApplyVulnerable, new[] {"NumberOfTurns"}},
        {EffectType.RemoveDebuffs, new string[0]},
        {EffectType.ShieldRemoveAll, new string[0]},
        {EffectType.ShieldFormula, new [] {"Formula"}},
        {EffectType.AdjustStatAdditivelyFormula, new [] { "Formula", "NumberOfTurns", "EffectScope" }},
        {EffectType.AdjustStatMultiplicativelyFormula, new [] { "Formula", "NumberOfTurns", "EffectScope" }},
        {EffectType.AdjustCounterFormula, new [] {"Formula", "EffectScope" }},
        {EffectType.GainCredits, new [] {"BaseAmount" }},
        {EffectType.AdjustPrimaryResource, new [] {"BaseAmount" }},
        {EffectType.AdjustPrimaryResourceFormula, new [] {"Formula"}},
        {EffectType.AdjustPlayerStats, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.AtStartOfTurn, new [] { "NumberOfTurns", "EffectScope", "ReferencedSequence", "StatusTag", "StatusDetailText" }},
        {EffectType.AtEndOfTurn, new [] { "NumberOfTurns", "EffectScope", "ReferencedSequence", "StatusTag", "StatusDetailText" }},
        {EffectType.DuplicateStatesOfType, new [] { "StatusTag" }},
        {EffectType.DealRawDamageFormula, new [] { "Formula" }},
        {EffectType.HealFormula, new [] { "Formula" }},
        {EffectType.ApplyAdditiveStatInjury, new [] { "FlavorText", "FloatAmount", "EffectScope" }},
        {EffectType.ApplyMultiplicativeStatInjury, new [] {  "FlavorText", "FloatAmount", "EffectScope" }},
        {EffectType.Kill, new string[0]},
        {EffectType.ShowCustomTooltip, new [] { "FlavorText", "FloatAmount", "EffectScope", "NumberOfTurns" }},
        {EffectType.OnDeath, new []{ "FloatAmount", "NumberOfTurns", "ReactionSequence",  }},
        {EffectType.PlayBonusCardAfterNoCardPlayedInXTurns, new[]{"BaseAmount", "EffectScope", "StatusTag", "StatusDetailText", "BonusCardType"}},
        {EffectType.AttackFormula, new [] { "Formula", "HitsRandomTargetMember" }},
        {EffectType.MagicAttackFormula, new [] { "Formula", "HitsRandomTargetMember" }},
        {EffectType.AddToXCostTransformer, new [] { "FloatAmount", "NumberOfTurns", "StatusTag", "StatusDetailText" }},
        {EffectType.RedrawHandOfCards, new string[0] },
        {EffectType.DrawCards, new [] { "Formula" } },
        {EffectType.GlitchRandomCards, new []{ "BaseAmount", "EffectScope" }},
        {EffectType.LeaveBattle, new string[0]},
        {EffectType.ResetStatToBase, new [] { "EffectScope" }},
        {EffectType.TransferPrimaryResourceFormula, new [] { "Formula" } },
        {EffectType.AdjustCardTagPrevention, new []{ "BaseAmount", "EffectScope" }},
        {EffectType.Reload, new string[0]},
        {EffectType.DamageOverTimeFormula, new [] { "Formula", "NumberOfTurns" }}
    };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("EffectType"));
        foreach (var prop in _globalProperties)
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(prop), true) + 2;
        foreach (var prop in _relevantProperties[GetEffectType(property)])
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(prop), true) + 2;
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = 16;
        var effectType = property.FindPropertyRelative("EffectType");
        EditorGUI.PropertyField(position, effectType);
        position.y += EditorGUI.GetPropertyHeight(effectType) + 2;
        var properties = _relevantProperties[GetEffectType(property)];
                
        for (var i = 0; i < _globalProperties.Count; i++)
        {
            var prop = property.FindPropertyRelative(_globalProperties[i]);
            var height = EditorGUI.GetPropertyHeight(prop);
            position.height = height;
            EditorGUI.PropertyField(position, prop);
            position.y += height + 2;
        }
        
        for (var i = 0; i < properties.Length; i++)
        {
            var prop = property.FindPropertyRelative(properties[i]);
            var height = EditorGUI.GetPropertyHeight(prop);
            position.height = height;
            EditorGUI.PropertyField(position, prop, true);
            position.y += height + 2;
        }
    }

    private EffectType GetEffectType(SerializedProperty property)
    {
        var effectType = property.FindPropertyRelative("EffectType");
        try
        {
            return (EffectType) Enum.GetValues(typeof(EffectType)).GetValue(effectType.enumValueIndex);
        }
        catch
        {
            return EffectType.Nothing;
        }
    }
}

#endif
