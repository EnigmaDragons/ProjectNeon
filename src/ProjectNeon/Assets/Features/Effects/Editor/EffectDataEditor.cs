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
        {EffectType.ReactOnEvadedWithCard, new []{ "FloatAmount", "NumberOfTurns", "EffectScope", "StatusTag", "StatusDetailText", "ReactionSequence" }},
        {EffectType.ReactOnSpellshieldedWithCard, new []{ "FloatAmount", "NumberOfTurns", "EffectScope", "StatusTag", "StatusDetailText", "ReactionSequence" }},
        {EffectType.ReactWithEffect, new []{ "FloatAmount", "NumberOfTurns", "EffectScope", "StatusTag", "StatusDetailText", "ReactionConditionType", "ReactionEffect"}},
        {EffectType.ReactWithCard, new []{ "FloatAmount", "NumberOfTurns", "EffectScope", "StatusTag", "StatusDetailText", "ReactionConditionType", "ReactionSequence"}},
        {EffectType.ResourceFlat, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.ApplyVulnerable, new[] {"NumberOfTurns"}},
        {EffectType.RemoveDebuffs, new string[0]},
        {EffectType.RemoveShields, new string[0]},
        {EffectType.AdjustStatAdditivelyFormula, new [] { "Formula", "NumberOfTurns", "EffectScope" }},
        {EffectType.AdjustStatMultiplicatively, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.AdjustCounter, new [] {"BaseAmount", "EffectScope" }},
        {EffectType.AdjustCounterFormula, new [] {"Formula", "EffectScope" }},
        {EffectType.GainCredits, new [] {"BaseAmount" }},
        {EffectType.AdjustPrimaryResource, new [] {"BaseAmount" }},
        {EffectType.AdjustPlayerStats, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" }},
        {EffectType.AtStartOfTurn, new [] { "NumberOfTurns", "EffectScope", "ReferencedSequence", "StatusTag", "StatusDetailText" }},
        {EffectType.AtEndOfTurn, new [] { "NumberOfTurns", "EffectScope", "ReferencedSequence", "StatusTag", "StatusDetailText" }},
        {EffectType.DuplicateStatesOfType, new [] { "StatusTag" }},
        {EffectType.DamageOverTime, new [] { "FloatAmount", "NumberOfTurns", "EffectScope" } },
        {EffectType.DealRawDamageFormula, new [] { "Formula" }},
        {EffectType.HealFormula, new [] { "Formula" }},
        {EffectType.ApplyAdditiveStatInjury, new [] { "FlavorText", "FloatAmount", "EffectScope" }},
        {EffectType.ApplyMultiplicativeStatInjury, new [] {  "FlavorText", "FloatAmount", "EffectScope" }},
        {EffectType.Kill, new string[0]},
        {EffectType.ShowCustomTooltip, new [] { "FlavorText", "FloatAmount", "EffectScope", "NumberOfTurns" }},
        {EffectType.OnDeath, new []{ "FloatAmount", "NumberOfTurns", "ReactionSequence" }},
        {EffectType.PlayBonusCardAfterNoCardPlayedInXTurns, new[]{"BaseAmount", "EffectScope", "StatusTag", "StatusDetailText", "BonusCardType"}},
        {EffectType.AttackFormula, new [] { "Formula" }},
        {EffectType.MagicAttackFormula, new [] { "Formula" }},
        {EffectType.AddToXCostTransformer, new [] { "FloatAmount", "NumberOfTurns", "StatusTag", "StatusDetailText" }},
        {EffectType.RedrawHandOfCards, new string[0] }
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
            EditorGUI.PropertyField(position, prop);
            position.y += EditorGUI.GetPropertyHeight(effectType) + 2;
        }
        
        for (var i = 0; i < properties.Length; i++)
        {
            var prop = property.FindPropertyRelative(properties[i]);
            EditorGUI.PropertyField(position, prop, true);
            position.y += EditorGUI.GetPropertyHeight(effectType) + 2;
        }
    }

    private EffectType GetEffectType(SerializedProperty property)
    {
        var effectType = property.FindPropertyRelative("EffectType");
        return (EffectType)Enum.GetValues(typeof(EffectType)).GetValue(effectType.enumValueIndex);
    }
}

#endif
